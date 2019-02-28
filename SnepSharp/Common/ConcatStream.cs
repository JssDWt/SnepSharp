//
//  ConcatStream.cs
//
//  Author:
//       Jesse de Wit <witdejesse@hotmail.com>
//
//  Copyright (c) 2019 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace SnepSharp.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// Concatenates multiple streams, providing an interface as if it were one stream.
    /// </summary>
    public class ConcatStream : Stream
    {
        /// <summary>
        /// The current stream position.
        /// </summary>
        private long position;

        /// <summary>
        /// The total length of the inner streams combined.
        /// </summary>
        private readonly long totalLength;

        /// <summary>
        /// The index of the stream that is currently being read.
        /// </summary>
        private int streamIndex;

        /// <summary>
        /// Value indicating whether the current object has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The inner streams.
        /// </summary>
        private List<Stream> innerStreams;

        /// <summary>
        /// Gets or sets a value indicating whether the inner streams should be disposed when this <see cref="T:Snep.ConcatStream"/> is disposed.
        /// </summary>
        /// <value><c>true</c> if dispose inner; otherwise, <c>false</c>.</value>
        public bool DisposeInner { get; set; } = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.ConcatStream"/> class, concatenating the provided streams in order.
        /// </summary>
        /// <param name="first">The first inner stream.</param>
        /// <param name="second">The second inner stream.</param>
        /// <param name="more">Optional extra inner streams.</param>
        public ConcatStream(Stream first, Stream second, params Stream[] more)
        {
            if (first == null) throw new ArgumentNullException(nameof(first));
            if (first == second) throw new ArgumentNullException(nameof(second));

            this.innerStreams = new List<Stream>
            {
                first,
                second
            };

            if (more != null)
            {
                foreach (Stream extra in more.Where(s => s != null))
                {
                    innerStreams.Add(extra);
                }
            }

            foreach (var stream in this.innerStreams)
            {
                if (!stream.CanRead) throw new ArgumentException("All streams must be readable.");
                if (!stream.CanSeek) throw new ArgumentException("All streams must be seekable, in order to establish the total length.");
            }

            this.totalLength = this.innerStreams.Sum(s => s.Length);
        }

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Snep.ByteHeaderStream"/> can be read.
        /// </summary>
        /// <value>returns <c>true</c>.</value>
        public override bool CanRead => true;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Snep.ByteHeaderStream"/> is seekable.
        /// </summary>
        /// <value>returns <c>true</c>.</value>
        public override bool CanSeek => true;

        /// <summary>
        /// Gets a value indicating whether this <see cref="T:Snep.ByteHeaderStream"/> can be written to.
        /// </summary>
        /// <value>returns <c>false</c>.</value>
        public override bool CanWrite => false;

        /// <summary>
        /// Gets the length of the stream, which is the combined length of the inner streams.
        /// </summary>
        /// <value>The length of the stream.</value>
        public override long Length => this.totalLength;

        /// <summary>
        /// Gets or sets the position of the stream.
        /// </summary>
        /// <value>The position.</value>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the position is outside the stream bounds.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Position
        {
            get => this.position;
            set
            {
                this.ThrowIfDisposed();
                if (value < 0 || value > this.totalLength)
                {
                    throw new ArgumentOutOfRangeException(nameof(Position));
                }

                this.position = value;
                long currentPosition = 0;
                    
                for (int i = 0; i < this.innerStreams.Count; i++)
                {
                    Stream currentStream = this.innerStreams[i];
                    if (currentPosition > this.position)
                    {
                        // This stream starts after the position.
                        currentStream.Seek(0, SeekOrigin.Begin);
                    }
                    else if (currentPosition + currentStream.Length > this.position)
                    {
                        // This stream 'contains' the position.
                        currentStream.Seek(this.position - currentPosition, SeekOrigin.Begin);
                        this.streamIndex = i;
                    }
                    else
                    {
                        // This stream is before the position.
                        currentStream.Seek(0, SeekOrigin.End);
                    }

                    currentPosition += currentStream.Length;
                }
            }
        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override void Flush() => throw new NotSupportedException();

        /// <summary>
        /// Reads a sequence of bytes from the underlying streams, in order and advances the position within the underlying streams by the number of bytes read.
        /// </summary>
        /// <returns>The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.</returns>
        /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
        /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
        /// <exception cref="ArgumentException">The sum of offset and count is larger than the buffer length.</exception>
        /// <exception cref="ArgumentNullException">buffer is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">offset or count is negative.</exception>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override int Read(byte[] buffer, int offset, int count)
        {
            this.ThrowIfDisposed();
            if (buffer == null) throw new ArgumentNullException(nameof(buffer));
            if (offset + count > buffer.Length) throw new ArgumentException("The sum of offset and count is larger than the buffer length.");
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset));
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count));

            int bytesRead = this.innerStreams[this.streamIndex].Read(buffer, offset, count);
            if (bytesRead < count && this.streamIndex < this.innerStreams.Count)
            {
                this.streamIndex++;
                bytesRead += this.Read(buffer, offset + bytesRead, count - bytesRead);
            }

            return bytesRead;
        }

        /// <summary>
        /// Sets the position somewhere within the underlying streams.
        /// </summary>
        /// <returns>The new position within the current stream.</returns>
        /// <param name="offset">A byte offset relative to the origin parameter.</param>
        /// <param name="origin">A value of type SeekOrigin indicating the reference point used to obtain the new position.</param>
        /// <exception cref="ObjectDisposedException">Methods were called after the stream was closed.</exception>
        public override long Seek(long offset, SeekOrigin origin)
        {
            this.ThrowIfDisposed();

            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    this.Position = this.Length + offset;
                    break;
                default:
                    throw new ArgumentException("Unknown SeekOrigin");
            }

            return this.Position;
        }

        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <summary>
        /// Releases all underlying streams.
        /// </summary>
        /// <param name="disposing">Value indicating whether the current object is being disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.innerStreams != null && this.DisposeInner)
            {
                foreach (var stream in this.innerStreams)
                {
                    stream.Close();
                }

                this.innerStreams = null;
                this.isDisposed = true;
            }

            base.Dispose(disposing);
        }

        /// <summary>
        /// Throws an <see cref="ObjectDisposedException"/> if the current object has been disposed.
        /// </summary>
        private void ThrowIfDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(nameof(ConcatStream));
            }
        }
    }
}

namespace SnepSharp.Common
{
    using System;
    using System.IO;

    /// <summary>
    /// Represents a stream with a byte header. The stream will be read as if the byte header is part of the stream itself.
    /// </summary>
    public class ByteHeaderStream : Stream
    {
        /// <summary>
        /// The current stream position.
        /// </summary>
        private long position;

        /// <summary>
        /// Value indicating whether the inner stream should be disposed when the current stream is being disposed.
        /// </summary>
        private readonly bool disposeInner;

        /// <summary>
        /// Value indicating whether the current stream has been disposed.
        /// </summary>
        private bool isDisposed;

        /// <summary>
        /// The inner stream.
        /// </summary>
        private Stream inner;

        /// <summary>
        /// The header that will be put before the inner stream.
        /// </summary>
        private readonly byte[] header;

        /// <summary>
        /// The offset to start reading the header.
        /// </summary>
        private readonly long headerOffset;

        /// <summary>
        /// The amount of bytes to read from the header.
        /// </summary>
        private readonly long headerCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.ByteHeaderStream"/> class.
        /// </summary>
        /// <param name="inner">The inner stream.</param>
        /// <param name="header">The header that will be put before the inner stream.</param>
        /// <param name="disposeInner">If set to <c>true</c> inner stream will be disposed when the current stream is disposed.</param>
        public ByteHeaderStream(Stream inner, byte[] header, bool disposeInner = false)
            : this(inner, header, 0, header.Length, disposeInner)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Snep.ByteHeaderStream"/> class, using part of the header array as header.
        /// </summary>
        /// <param name="inner">The inner stream.</param>
        /// <param name="header">The header that will be put before the inner stream.</param>
        /// <param name="offset">Offset to start reading the header from.</param>
        /// <param name="count">Amount of bytes to read from the header.</param>
        /// <param name="disposeInner">If set to <c>true</c> inner stream will be disposed when the current stream is disposed.</param>
        public ByteHeaderStream(Stream inner, byte[] header, long offset, long count, bool disposeInner = false)
        {
            if (inner == null) throw new ArgumentNullException(nameof(inner));
            if (header == null) throw new ArgumentNullException(nameof(header));
            if (!inner.CanRead) throw new ArgumentException("Inner stream must be readable.", nameof(inner));
            if (!inner.CanSeek) throw new ArgumentException("Inner stream must be seekable.", nameof(inner));
            if (offset < 0) throw new ArgumentOutOfRangeException(nameof(offset), "offset cannot be negative.");
            if (count < 0) throw new ArgumentOutOfRangeException(nameof(count), "count cannot be negative.");
            if (offset + count > header.Length) throw new ArgumentException("offset + count cannot be larger than the header length.");

            this.inner = inner;
            this.header = header;
            this.headerOffset = offset;
            this.headerCount = count;
            this.disposeInner = disposeInner;
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
        /// Gets the length of the stream.
        /// </summary>
        /// <value>The length of the stream.</value>
        public override long Length => this.headerCount + this.inner.Length;

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

                if (value < 0 || value > this.Length)
                {
                    throw new ArgumentOutOfRangeException(nameof(Position));
                }

                this.position = value;
                this.inner.Seek(this.position - this.headerCount, SeekOrigin.Begin);
            }

        }

        /// <summary>
        /// Not supported.
        /// </summary>
        public override void Flush() => throw new NotSupportedException();

        /// <summary>
        /// Reads a sequence of bytes from the stream, starting with the header, and advances the position within the stream by the number of bytes read.
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

            int bytesRead = 0;
            if (this.position < this.headerCount)
            {
                int copyAmount = (int)Math.Min(this.headerCount - this.position, count);
                Array.Copy(this.header, this.position + this.headerOffset, buffer, offset, copyAmount);
                bytesRead += copyAmount;
                this.position += copyAmount;
            }

            if (bytesRead < count)
            {
                bytesRead += this.inner.Read(buffer, offset + bytesRead, count - bytesRead);
            }

            this.position += bytesRead;
            return bytesRead;
        }

        /// <summary>
        /// Sets the position within the stream.
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

        /// <summary>
        /// Not supported
        /// </summary>
        /// <param name="value">Value.</param>
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        /// <summary>
        /// Releases the inner stream.
        /// </summary>
        /// <param name="disposing">Value indicating whether the current object is being disposed.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && this.inner != null && this.disposeInner)
            {
                this.inner.Close();
                this.inner = null;
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

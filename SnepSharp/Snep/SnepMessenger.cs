﻿//
//  SnepMessenger.cs
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

namespace SnepSharp.Snep
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading;
    using System.Threading.Tasks;
    using SnepSharp.Llcp;
    using SnepSharp.Ndef;
    using SnepSharp.Snep.Messages;

    /// <summary>
    /// Snep messenger, handles snep message fragmentation and continue 
    /// responses.
    /// </summary>
    internal class SnepMessenger : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this is a client messenger.
        /// </summary>
        /// <value><c>true</c> if is client messages; <c>false</c> if is server 
        /// messenger.</value>
        public bool IsClient { get; }

        /// <summary>
        /// The Llcp socket to send messages over.
        /// </summary>
        private ISocket socket;

        /// <summary>
        /// Handles to returned <see cref="SnepMessageStream"/>. They will be 
        /// disposed when the current messenger is closed.
        /// </summary>
        private List<SnepMessageStream> messageStreams 
            = new List<SnepMessageStream>();

        /// <summary>
        /// The ndef message parser.
        /// </summary>
        private INdefParser ndefParser;

        /// <summary>
        /// The maximum size of the content, above which the message will be 
        /// rejected.
        /// </summary>
        private readonly int maxReceiveContentSize;

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:SnepSharp.Snep.SnepMessenger"/> class.
        /// </summary>
        /// <param name="isClient"><c>true</c> if client, <c>false</c> is 
        /// server.</param>
        public SnepMessenger(
            bool isClient, 
            ISocket socket, 
            INdefParser ndefParser, 
            int maxReceiveContentSize)
        {
            this.socket = socket 
                ?? throw new ArgumentNullException(nameof(socket));
            this.ndefParser = ndefParser 
                ?? throw new ArgumentNullException(nameof(ndefParser));
            this.IsClient = isClient;
            this.maxReceiveContentSize = maxReceiveContentSize;
        }

        /// <summary>
        /// Sends the specified message over the socket.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <exception cref="T:SnepSharp.Snep.SnepException">Thrown when the 
        /// other party does not return a continue response in a fragmented 
        /// message.</exception>
        public async Task SendMessage(SnepMessage message)
        {
            using (var messageStream = message.AsStream())
            {
                int firstFragmentLength = Math.Min(
                    socket.LocalMiu, 
                    message.MessageLength);
                var sendBuffer = new byte[firstFragmentLength];
                int bytesRead = messageStream.Read(sendBuffer, 0, firstFragmentLength);
                await socket.Send(sendBuffer, bytesRead);

                if (firstFragmentLength == message.MessageLength)
                {
                    return;
                }

                // If we arrive here, the message to send is fragmented.
                // Wait for continue. 
                // TODO: pass in cancellationtoken.
                var receiveBuffer = await socket.Receive();
                var response = SnepMessage.Parse(
                    receiveBuffer, 
                    this.ndefParser);

                // Request and response 'Continue' are different codes.
                // Handle them seperately
                if (message is SnepRequest)
                {
                    if (response.Header.Command != (byte)SnepResponseCode.Continue)
                    {
                        string exmsg = $"Got invalid snep response code '" +
                            response.Header.Command.ToString("X2") + "', expected '" +
                            ((byte)SnepResponseCode.Continue).ToString("X2") +
                            "' (Continue).";
                        throw new SnepException(exmsg);
                    }
                }
                else // message is SnepResponse
                {
                    if (response.Header.Command != (byte)SnepRequestCode.Continue)
                    {
                        string exmsg = $"Got invalid snep request code '" +
                            response.Header.Command.ToString("X2") + "', expected '" +
                            ((byte)SnepRequestCode.Continue).ToString("X2") +
                            "' (Continue).";
                        throw new SnepException(exmsg);
                    }
                }

                int offset = firstFragmentLength;

                while (offset < message.MessageLength || bytesRead == 0)
                {
                    int currentFragmentSize = Math.Min(socket.LocalMiu, message.MessageLength - offset);
                    bytesRead = messageStream.Read(sendBuffer, 0, currentFragmentSize);
                    await socket.Send(sendBuffer, bytesRead);
                    offset += currentFragmentSize;
                }
            }
        }

        /// <summary>
        /// Gets a snep message over the llcp socket.
        /// </summary>
        /// <returns>The received message.</returns>
        /// <remarks>If the <see cref="SnepMessage"/> would exceed the 
        /// <see cref="INdefParser.MaxBufferSize"/>, the message is dragged in 
        /// while the message is being read. If the current 
        /// <see cref="SnepMessenger"/> is disposed, the underlying 
        /// <see cref="Stream"/> of the <see cref="SnepMessage"/> is also 
        /// disposed. So do not dispose the <see cref="SnepMessenger"/> before 
        /// reading the returned messages.</remarks>
        public async Task<SnepMessage> GetMessage(CancellationToken token)
        {
            // TODO: support cancellation.
            var receiveBuffer = await socket.Receive();
            int bytesReceived = receiveBuffer.Length;

            if (bytesReceived < Constants.SnepHeaderLength)
            {
                SnepMessage rejection = this.IsClient 
                    ? (SnepMessage)new SnepRejectRequest() 
                    : new SnepRejectResponse();

                try
                {
                    var sendBuffer = new byte[Constants.SnepHeaderLength];
                    using (var s = rejection.AsStream())
                    {
                        s.Read(sendBuffer, 0, Constants.SnepHeaderLength);
                        await socket.Send(sendBuffer, sendBuffer.Length, token);
                    }
                }
                catch
                {
                    // squelch
                }

                throw new SnepException("Received invalid fragment from sender.");
            }

            var header = SnepHeader.FromBytes(receiveBuffer);
            if (header.Version != Constants.DefaultSnepVersion)
            {
                // Unsupported version, consider the message complete.
                return SnepMessage.FromHeader(header);
            }

            if (header.ContentLength + Constants.SnepHeaderLength >= bytesReceived)
            {
                // Full message obtained.
                return SnepMessage.Parse(
                    receiveBuffer, 
                    bytesReceived,
                    this.ndefParser);
            }

            if (header.ContentLength > this.maxReceiveContentSize)
            {
                // Reject too large messages.
                SnepMessage rejection = this.IsClient
                    ? (SnepMessage)new SnepRejectRequest()
                    : new SnepRejectResponse();

                try
                {
                    var sendBuffer = new byte[Constants.SnepHeaderLength];
                    using (var s = rejection.AsStream())
                    {
                        s.Read(sendBuffer, 0, Constants.SnepHeaderLength);
                        await socket.Send(sendBuffer, sendBuffer.Length, token);
                    }
                }
                catch
                {
                    // squelch
                }
            }

            // Send continue in order to start fetching the next fragments.
            SnepMessage continu = this.IsClient 
                ? (SnepMessage)new SnepContinueRequest()
                : new SnepContinueResponse();
            using (var s = continu.AsStream())
            {
                var sendBuffer = new byte[Constants.SnepHeaderLength];
                s.Read(sendBuffer, 0, Constants.SnepHeaderLength);
                await socket.Send(sendBuffer, sendBuffer.Length, token);
            }

            SnepMessage result;
            if (header.ContentLength > this.ndefParser.MaxBufferSize)
            {
                // This message exceeds the buffer size, so drag the message in
                // as it is read by the client application.
                var content = new SnepMessageStream(
                    header, 
                    receiveBuffer, 
                    socket);
                this.messageStreams.Add(content);
                result = SnepMessage.FromStream(
                    header,
                    content,
                    this.ndefParser);
            }
            else
            {
                // If the message is not large, handle it in memory.
                var message = new MemoryStream();
                message.Write(
                    receiveBuffer,
                    0,
                    receiveBuffer.Length);

                while (bytesReceived < 
                    header.ContentLength + Constants.SnepHeaderLength)
                {
                    // TODO: Support cancellation.
                    var currentBuffer = await socket.Receive(token);
                    bytesReceived += currentBuffer.Length;
                    message.Write(currentBuffer, 0, currentBuffer.Length);
                }

                result = SnepMessage.Parse(
                    message.ToArray(),
                    bytesReceived,
                    this.ndefParser);
            }

            return result;
        }

        /// <summary>
        /// Close the messenger and the underlying socket.
        /// </summary>
        public void Close()
        {
            if (this.messageStreams != null)
            {
                // Close any message streams that are using the same socket.
                foreach (var s in this.messageStreams)
                {
                    if (!s.isDisposed)
                    {
                        s.Dispose();
                    }
                }

                this.messageStreams = null;
            }

            if (this.socket != null)
            {
                this.socket.Close();
                this.socket = null;
            }
        }

        /// <summary>
        /// Calls <see cref="Close"/>.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Close();
        }

        /// <summary>
        /// Snep message stream, drags data in while it is being read.
        /// </summary>
        private class SnepMessageStream : Stream
        {
            /// <summary>
            /// Value indicating whether the current object has been disposed.
            /// </summary>
            public bool isDisposed;

            /// <summary>
            /// The snep header.
            /// </summary>
            private readonly SnepHeader header;

            /// <summary>
            /// The socket to drag in more data.
            /// </summary>
            private ISocket socket;

            /// <summary>
            /// The internal buffer offset.
            /// </summary>
            private int internalOffset;

            /// <summary>
            /// The amount of bytes left in the internal buffer.
            /// </summary>
            private int internalCount;

            /// <summary>
            /// The internal buffer.
            /// </summary>
            private byte[] internalBuffer;

            /// <summary>
            /// The amount of bytes received over llcp.
            /// </summary>
            private int contentBytesReceived;

            /// <summary>
            /// Value indicating whether the end of the snep message was reached.
            /// </summary>
            private bool endReached;

            /// <summary>
            /// Initializes a new instance of the <see cref="T:SnepSharp.Snep.SnepMessenger.SnepMessageStream"/> class.
            /// </summary>
            /// <param name="initialFragment">Initial fragment.</param>
            /// <param name="socket">Llcp socket.</param>
            public SnepMessageStream(SnepHeader header, byte[] initialFragment, ISocket socket)
            {
                this.header = header;
                this.socket = socket;
                this.internalBuffer = initialFragment;
                this.internalOffset = Constants.SnepHeaderLength;
                this.internalCount = initialFragment.Length - this.internalOffset;
                this.contentBytesReceived = initialFragment.Length - Constants.SnepHeaderLength;
            }

            /// <summary>
            /// Reads a sequence of bytes from the current stream and advances 
            /// the position within the stream by the number of bytes read. 
            /// </summary>
            /// <remarks>The data is being dragged in over llcp while the stream 
            /// is read. This way the buffer size will not be large when large
            /// content is being transferred. However, reads will be slower. This
            /// Stream is only used when the snep message would otherwise exceed 
            /// the max buffer size.</remarks>
            /// <returns>The read.</returns>
            /// <param name="buffer">An array of bytes. When this method returns, 
            /// the buffer contains the specified byte array with the values 
            /// between offset and (offset + count - 1) replaced by the bytes 
            /// read from the current source.</param>
            /// <param name="offset">The zero-based byte offset in buffer at 
            /// which to begin storing the data read from the current stream.</param>
            /// <param name="count">The maximum number of bytes to be read from 
            /// the current stream.</param>
            public override int Read(byte[] buffer, int offset, int count)
            {
                if (this.isDisposed)
                {
                    throw new ObjectDisposedException(nameof(SnepMessageStream));
                }

                if (buffer == null) throw new ArgumentNullException(nameof(buffer));
                if (offset + count > buffer.Length)
                {
                    throw new ArgumentException(
                        "The sum of offset and count is larger than the buffer length.");
                }

                if (offset < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(offset), 
                        "offset cannot be negative");
                }

                if (count < 0)
                {
                    throw new ArgumentOutOfRangeException(
                        nameof(count),
                        "count cannot be negative");
                }

                // Empty the internal buffer first.
                int currentCount = Math.Min(count, internalCount);
                Array.Copy(this.internalBuffer, this.internalOffset, buffer, offset, currentCount);

                // If we get here, refill the buffer over llcp.
                while (!this.endReached && currentCount < count)
                {
                    // TODO: support cancellation.
                    // TODO: Make async.
                    this.internalBuffer = socket.Receive().Result;
                    int currentBytes = this.internalBuffer.Length;
                    this.contentBytesReceived += currentBytes;
                    this.internalOffset = 0;
                    this.internalCount = currentBytes;
                    if (this.contentBytesReceived >= this.header.ContentLength)
                    {
                        endReached = true;
                    }

                    int extraCount = Math.Min(count - currentCount, internalCount);
                    Array.Copy(this.internalBuffer, this.internalOffset, buffer, offset + currentCount, extraCount);
                    currentCount += extraCount;
                }

                return currentCount;
            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position
            {
                get => this.contentBytesReceived - this.internalCount + Constants.SnepHeaderLength;
                set => throw new NotSupportedException();
            }
            public override void Flush() => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                this.socket = null;
                this.isDisposed = true;
            }
        }
    }
}

namespace SnepSharp.Snep
{
    using System;
    using System.IO;
    using SnepSharp.Llcp;
    using SnepSharp.Snep.Messages;

    internal class SnepMessenger : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this is a client messenger.
        /// </summary>
        /// <value><c>true</c> if is client messages; <c>false</c> if is server messenger.</value>
        public bool IsClient { get; }

        /// <summary>
        /// The Llcp socket to send messages over.
        /// </summary>
        private LogicalLinkControl socket;

        private byte RemoteContinue;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SnepSharp.Snep.SnepMessenger"/> class.
        /// </summary>
        /// <param name="isClient"><c>true</c> if client, <c>false</c> is server.</param>
        public SnepMessenger(bool isClient, LogicalLinkControl socket)
        {
            this.socket = socket ?? throw new ArgumentNullException(nameof(socket));
            this.IsClient = isClient;
        }

        public void SendMessage(SnepMessage message)
        {
            using (var messageStream = message.AsStream())
            {
                int firstFragmentLength = Math.Min(socket.MaximumInformationUnit, message.MessageLength);
                var sendBuffer = new byte[firstFragmentLength];
                int bytesRead = messageStream.Read(sendBuffer, 0, firstFragmentLength);
                socket.Send(sendBuffer, bytesRead);

                if (firstFragmentLength == message.MessageLength)
                {
                    return;
                }

                // If we arrive here, the message to send is fragmented.
                // Wait for continue.
                var receiveBuffer = new byte[this.socket.MaximumInformationUnit];
                int bytesReceived = socket.Receive(receiveBuffer);
                var response = SnepMessage.Parse(receiveBuffer, bytesReceived);

                // Request and response 'Continue' are different codes.
                // Handle them seperately
                if (message is SnepRequest)
                {
                    if (response.Command != (byte)SnepResponseCode.Continue)
                    {
                        string exmsg = $"Got invalid snep response code '" +
                            response.Command.ToString("X2") + "', expected '" +
                            ((byte)SnepResponseCode.Continue).ToString("X2") +
                            "' (Continue).";
                        throw new SnepException(exmsg);
                    }
                }
                else // message is SnepResponse
                {
                    if (response.Command != (byte)SnepRequestCode.Continue)
                    {
                        string exmsg = $"Got invalid snep request code '" +
                            response.Command.ToString("X2") + "', expected '" +
                            ((byte)SnepRequestCode.Continue).ToString("X2") +
                            "' (Continue).";
                        throw new SnepException(exmsg);
                    }
                }

                int offset = firstFragmentLength;

                while (offset < message.MessageLength || bytesRead == 0)
                {
                    int currentFragmentSize = Math.Min(socket.MaximumInformationUnit, message.MessageLength - offset);
                    bytesRead = messageStream.Read(sendBuffer, 0, currentFragmentSize);
                    socket.Send(sendBuffer, bytesRead);
                    offset += currentFragmentSize;
                }
            }
        }

        public SnepMessage GetMessage(int maxBufferSize = 16384)
        {
            var receiveBuffer = new byte[socket.MaximumInformationUnit];
            int bytesReceived = socket.Receive(receiveBuffer);

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
                        socket.Send(sendBuffer);
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
                return SnepMessage.Parse(receiveBuffer, bytesReceived);
            }

            // Send continue in order to start fetching the next fragments.
            SnepMessage continu = this.IsClient 
                ? (SnepMessage)new SnepContinueRequest()
                : new SnepContinueResponse();
            using (var s = continu.AsStream())
            {
                var sendBuffer = new byte[Constants.SnepHeaderLength];
                s.Read(sendBuffer, 0, Constants.SnepHeaderLength);
                socket.Send(sendBuffer);
            }

            Stream content;
            if (header.ContentLength > maxBufferSize)
            {
                // This message exceeds the buffer size, so drag the message in
                // as it is read by the client application.
                content = new SnepMessageStream(receiveBuffer, socket, maxBufferSize);
            }
            else
            {
                content = new MemoryStream();
                content.Write(
                    receiveBuffer,
                    Constants.SnepHeaderLength,
                    receiveBuffer.Length - Constants.SnepHeaderLength);

                while (bytesReceived < header.ContentLength + Constants.SnepHeaderLength)
                {
                    int currentBytes = socket.Receive(receiveBuffer);
                    bytesReceived += currentBytes;
                    content.Write(receiveBuffer, 0, currentBytes);
                }

                content.Seek(0, SeekOrigin.Begin);
            }

            return SnepMessage.FromNdef(header, NdefMessage.FromStream(content));
        }

        public void Close()
        {
            this.socket.Close();
        }

        void IDisposable.Dispose()
        {
            this.Close();
        }

        private class SnepMessageStream : Stream
        {
            private int position = Constants.SnepHeaderLength;
            private byte[] initialFragment;
            public SnepMessageStream(byte[] initialFragment, LogicalLinkControl socket, int maxBufferSize)
            {
                this.initialFragment = initialFragment;
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
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


            }

            public override bool CanRead => true;
            public override bool CanSeek => false;
            public override bool CanWrite => false;
            public override long Length => throw new NotSupportedException();
            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }
            public override void Flush() => throw new NotSupportedException();
            public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
            public override void SetLength(long value) => throw new NotSupportedException();
            public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();
        }
    }
}

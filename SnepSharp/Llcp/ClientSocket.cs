//
//  ClientSocket.cs
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
using System;
using System.Collections.Concurrent;
using System.Threading;
using SnepSharp.Llcp.Pdus;

namespace SnepSharp.Llcp
{
    public class ClientSocket : IDisposable
    {
        private class PendingSend
        {
            public ManualResetEventSlim ManualResetEvent { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            public Exception InnerException { get; set; }
        }

        private object sendLock = new object();
        private object receiveLock = new object();
        private ManualResetEventSlim receiveAvailable 
            = new ManualResetEventSlim(false);
        private SemaphoreSlim receiveSemaphore = new SemaphoreSlim(1, 1);
        private ConcurrentQueue<byte[]> sendQueue 
            = new ConcurrentQueue<byte[]>();

        private ConcurrentQueue<PendingSend> pendingSendQueue 
            = new ConcurrentQueue<PendingSend>();

        private ConcurrentQueue<InformationUnit> receiveQueue
            = new ConcurrentQueue<InformationUnit>();

        public int LocalMiu { get; private set; }
        public int RemoteMiu { get; private set; } 
            = Constants.MaximumInformationUnit;
        public SocketState State { get; private set; } = SocketState.Closed;
        public int ReceiveBufferSize { get; private set; } = 1;
        public int SendBufferSize { get; private set; } = 1;

        public void Send(byte[] message, int count)
            => this.Send(message, count, new CancellationToken(false));

        public void Send(byte[] message, int count, CancellationToken token)
            => this.Send(message, 0, count, token);

        public void Send(byte[] message, int offset, int count)
            => this.Send(message, offset, count, new CancellationToken(false));

        public void Send(
            byte[] message,
            int offset,
            int count,
            CancellationToken token)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (offset < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    "Cannot be negative.");
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(offset),
                    "Cannot be negative.");
            }

            if (offset + count > message.Length)
            {
                throw new ArgumentException(
                    "offset + count cannot be greater than message length.");
            }

            if (count > this.RemoteMiu)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Message size exceeds send maximum information size.");
            }

            if (this.State != SocketState.Established)
            {
                if (this.State == SocketState.Closing)
                {
                    // TODO: throw error broken pipe
                }

                // TODO: throw error not connected.
            }

            var buffer = new byte[count];
            Array.Copy(message, offset, buffer, 0, count);

            using (var handle = new ManualResetEventSlim(false))
            {
                var pendingSend = new PendingSend
                {
                    ManualResetEvent = handle
                };

                lock (this.sendLock)
                {
                    this.sendQueue.Enqueue(buffer);
                    this.pendingSendQueue.Enqueue(pendingSend);
                }

                // Wait for just created the send handle to notify.
                handle.Wait(token);

                if (!pendingSend.Success)
                {
                    // TODO: Throw communication exception with message and inner exception.
                }
            }
        }

        internal byte[] DequeueForSend()
        {
            this.sendQueue.TryDequeue(out byte[] result);
            return result;
        }

        internal void SendNotification(
            bool success,
            string message,
            Exception inner)
        {
            if (!this.pendingSendQueue.TryDequeue(out PendingSend result))
            {
                throw new InvalidOperationException(
                    "No pending sends remaining.");
            }

            result.Success = success;
            result.Message = message;
            result.InnerException = inner;
            result.ManualResetEvent.Set();
        }

        public byte[] Receive() => this.Receive(new CancellationToken(false));

        public byte[] Receive(CancellationToken token)
        {
            // This makes sure clients fall through one by one.
            this.receiveSemaphore.Wait(token);

            try
            {
                // This makes sure a message is available.
                this.receiveAvailable.Wait(token);

                if (!this.receiveQueue.TryDequeue(out InformationUnit result))
                {
                    // TODO: Log a horrible bug has occurred.
                }

                lock (this.receiveLock)
                {
                    if (this.receiveQueue.Count == 0)
                    {
                        this.receiveAvailable.Reset();
                    }
                }

                return result.Information;
            }
            finally
            {
                // make space for the next receive
                this.receiveSemaphore.Release();
            }
        }

        internal void EnqueueReceived(ProtocolDataUnit receivePdu)
        {
            this.receiveQueue.Enqueue((InformationUnit)receivePdu);

            lock (this.receiveLock)
            {
                this.receiveAvailable.Set();
            }
        }

        /// <summary>
        /// Close the <see cref="DataLinkConnection"/>.
        /// </summary>
        /// <remarks>Closing the socket will not allow it to be reopened again.
        /// Closing the socket will leave ongoing send and receive operations in 
        /// an undefined state.</remarks>
        public void Close() => this.Dispose(true);

        /// <summary>
        /// Releases all managed and unmanaged resources.
        /// </summary>
        void IDisposable.Dispose() => this.Close();

        /// <summary>
        /// Releases all resources and sets the socket to a 
        /// </summary>
        /// <param name="disposing">If set to <c>true</c> disposing.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {

            }
        }
    }
}

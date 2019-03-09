//
//  DataLinkConnection.cs
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


namespace SnepSharp.Llcp
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using SnepSharp.Llcp.Exceptions;
    using SnepSharp.Llcp.Pdus;

    public class DataLinkConnection : ISocket, ILlcDispatch
    {
        private class PendingSend
        {
            public ManualResetEventSlim ManualResetEvent { get; set; }
            public bool Success { get; set; }
            public string Message { get; set; }
            public Exception InnerException { get; set; }
        }

        private readonly object sendLock = new object();
        private readonly object receiveLock = new object();
        private readonly ManualResetEventSlim receiveAvailable
            = new ManualResetEventSlim(false);
        private readonly SemaphoreSlim receiveSemaphore
            = new SemaphoreSlim(1, 1);

        private readonly ConcurrentQueue<byte[]> sendQueue
            = new ConcurrentQueue<byte[]>();
        private readonly ConcurrentQueue<ProtocolDataUnit> sendFastLane
            = new ConcurrentQueue<ProtocolDataUnit>();
        private readonly ConcurrentQueue<PendingSend> pendingSendQueue
            = new ConcurrentQueue<PendingSend>();
        private readonly ConcurrentQueue<ProtocolDataUnit> receiveQueue
            = new ConcurrentQueue<ProtocolDataUnit>();

        private LogicalLinkControl llc;

        /// <summary>
        /// The send state variable V(S). Denotes the sequence number of the 
        /// next in-sequence Information pdu to be sent over the data link
        /// connection.
        /// </summary>
        private SequenceNumber sendState;

        /// <summary>
        /// The receive state variable V(R). Denotes the sequence number of the 
        /// next in-sequence Information pdu to be received over the data link
        /// connection.
        /// </summary>
        private SequenceNumber receiveState;

        private SequencePair CurrentStateVariable 
            => new SequencePair(this.sendState, this.receiveState);

        /// <summary>
        /// The send acknowledgement state variable. Contains the most recently
        /// received Receive sequence number N(R).
        /// </summary>
        private SequenceNumber sendAckState;

        /// <summary>
        /// The receive acknowledgement state variable. Contains the most 
        /// recently sent Receive sequence number N(R). 
        /// </summary>
        private SequenceNumber receiveAckState;

        private SequencePair CurrentAckStateVariable
            => new SequencePair(this.sendAckState, this.receiveAckState);

        /// <summary>
        /// The amount of pdus received, but not yet received by the client
        /// application.
        /// </summary>
        private int pendingReceiveConfirmations = 0;

        /// <summary>
        /// Value indicating whether the remote peer is busy.
        /// </summary>
        private bool sendBusy = false;

        /// <summary>
        /// Gets the remote receive window size RW(R).
        /// </summary>
        public int RemoteReceiveWindowSize { get; internal set; }

        /// <summary>
        /// Gets the local receive window size RW(L).
        /// </summary>
        public int LocalReceiveWindowSize { get; internal set; }

        /// <summary>
        /// Gets the maximum size of the send buffer
        /// </summary>
        /// <value>The size of the send buffer.</value>
        public int SendBufferSize { get; internal set; }

        /// <summary>
        /// Gets the maximum size of the receive buffer.
        /// </summary>
        /// <value>The size of the receive buffer.</value>
        public int ReceiveBufferSize { get; internal set; }

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="DataLinkConnection"/> is bound to a local service access 
        /// point.
        /// </summary>
        /// <value><c>true</c> if is bound, otherwise <c>false</c>.</value>
        public bool IsBound => this.Address.HasValue;

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="DataLinkConnection"/> is connected to a remote peer.
        /// </summary>
        /// <value><c>true</c> if is connected; otherwise, <c>false</c>.</value>
        public bool IsConnected => this.IsBound && this.Peer.HasValue;

        /// <summary>
        /// Gets the remote maximum information unit size.
        /// </summary>
        /// <value>The remote miu.</value>
        public int RemoteMiu { get; internal set; }

        /// <summary>
        /// Gets the local maximum information unit size.
        /// </summary>
        /// <value>The local miu.</value>
        public int LocalMiu { get; }

        /// <summary>
        /// Gets the state of the socket.
        /// </summary>
        /// <value>The socket state.</value>
        public SocketState State { get; private set; }

        /// <summary>
        /// Gets the local service access point address for this connection.
        /// </summary>
        /// <value>The address.</value>
        public LinkAddress? Address
        {
            get;
            internal set; //TODO: Warn if socket gets rebound.
        }

        /// <summary>
        /// Gets the remote service access point address for this connection.
        /// </summary>
        /// <value>The peer.</value>
        public LinkAddress? Peer { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLinkConnection"/> 
        /// class.
        /// </summary>
        /// <param name="llc">Llc.</param>
        internal DataLinkConnection(LogicalLinkControl llc)
        {
            this.State = SocketState.Closed;
            this.llc = llc;
        }

        /// <summary>
        /// Binds the <see cref="DataLinkConnection"/> to the specified local 
        /// service access point address.
        /// </summary>
        /// <param name="address">The address to bind to.</param>
        public void Bind(LinkAddress address)
        {
            lock (this.sendLock)
            lock (this.receiveLock)
            {
                this.llc.Bind(this, address);
            }
        }

        /// <summary>
        /// Binds the <see cref="DataLinkConnection"/> to the specified local service name.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public void Bind(string serviceName)
        {
            lock (this.sendLock)
            lock (this.receiveLock)
            {
                this.llc.Bind(this, serviceName);
            }
        }

        /// <summary>
        /// Binds the <see cref="DataLinkConnection"/> to any available local service access 
        /// point address.
        /// </summary>
        public void Bind()
        {
            lock (this.sendLock)
            lock (this.receiveLock)
            {
                this.llc.Bind(this);
            }
        }

        public ProtocolDataUnit SendAcknowledgement()
        {
            throw new NotImplementedException();
        }

        public void Connect(LinkAddress destination)
            => this.Connect(destination, new CancellationToken(false));

        /// <summary>
        /// Connects the <see cref="DataLinkConnection"/> to the specified remote service
        /// access point address.
        /// </summary>
        /// <remarks>If the socket is not yet bound, the local address will be
        /// set to an available service access point.</remarks>
        /// <param name="destination">Destination service access point address.
        /// </param>
        public void Connect(LinkAddress destination, CancellationToken token)
        {
            this.Connect(() =>
            {
                var link = new DataLink(this.Address.Value, destination);
                return new ConnectUnit(
                    link,
                    this.LocalMiu,
                    this.LocalReceiveWindowSize);
            },
            token);
        }

        public void Connect(string serviceName)
            => this.Connect(serviceName, new CancellationToken(false));

        private void Connect(string serviceName, CancellationToken token)
        {
            this.Connect(() =>
            {
                var link = new DataLink(
                    this.Address.Value, 
                    LinkAddress.ServiceDiscoveryProtocolSap);
                return new ConnectUnit(
                    link,
                    this.LocalMiu,
                    this.LocalReceiveWindowSize,
                    serviceName);
            },
            token);
        }

        private void Connect(
            Func<ConnectUnit> createConnectPdu,
            CancellationToken token)
        {
            ConnectUnit connect = null;
            lock (this.sendLock)
            lock (this.receiveLock)
            {
                if (!this.IsBound)
                {
                    this.Bind();
                }

                if (this.State != SocketState.Closed)
                {
                    switch (this.State)
                    {
                        case SocketState.Established:
                            // TODO: Throw already connected.
                            break;
                        case SocketState.Connecting:
                            // TODO: Throw already connecting.
                            break;
                        default:
                            // TODO: Throw broken pipe.
                            break;
                    }
                }

                connect = createConnectPdu();
                this.State = SocketState.Connecting;
            }

            this.receiveSemaphore.Wait(token);

            try
            {
                using (var handle = new ManualResetEventSlim(false))
                {
                    var pendingSend = new PendingSend
                    {
                        ManualResetEvent = handle
                    };

                    lock (this.sendLock)
                    {
                        this.sendFastLane.Enqueue(connect);
                        this.pendingSendQueue.Enqueue(pendingSend);
                    }

                    // Wait for just created the send handle to notify.
                    // May throw operation cancelled, in which case the message is
                    // still enqueued... Maybe fix that later.
                    // TODO: Remove cancelled items from the send queue.
                    handle.Wait(token);

                    if (!pendingSend.Success)
                    {
                        // TODO: Throw more specific exception.
                        throw new CommunicationException(
                            pendingSend.Message,
                            pendingSend.InnerException);
                    }
                }

                this.receiveAvailable.Wait(token);

                lock (this.sendLock)
                lock (this.receiveLock)
                {
                    if (!this.receiveQueue.TryDequeue(
                        out ProtocolDataUnit response))
                    {
                        // TODO: Log a horrible bug has occurred, or the socket 
                        // was closed.
                        throw new OperationCanceledException(
                            "Socket was closed during operation.");
                    }

                    switch (response.Type)
                    {
                        case ProtocolDataUnitType.DisconnectedMode:
                            this.State = SocketState.Closed;
                            var dm = (DisconnectedModeUnit)response;
                            throw new ConnectionRefusedException(dm.Reason);
                        case ProtocolDataUnitType.ConnectionComplete:
                            var cc = (ConnectionCompleteUnit)response;
                            this.Peer = cc.DataLink.Source;
                            this.ReceiveBufferSize = this.LocalReceiveWindowSize;
                            this.RemoteMiu = cc.MaximumInformationUnit;
                            if (this.RemoteMiu > this.llc.SendMiu)
                            {
                                this.RemoteMiu = this.llc.SendMiu;
                            }
                            this.RemoteReceiveWindowSize = cc.ReceiveWindowSize;
                            this.State = SocketState.Established;
                            break;
                        default:
                            this.State = SocketState.Closed;
                            throw new ProtocolException(
                                $"Received unexpected pdu type " +
                                $"'{response.Type.ToString()}'");
                    }
                }
            }
            finally
            {
                this.receiveSemaphore.Release();
            }
        }

        public void Listen(int receiveBuffer)
        {
            if (receiveBuffer < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(receiveBuffer),
                    "Cannot be negative.");
            }

            receiveBuffer = Math.Min(receiveBuffer, 16);
            lock (this.sendLock)
            lock (this.receiveLock)
            {
                if (this.State != SocketState.Closed)
                {
                    if (this.State == SocketState.Shutdown)
                    {
                        throw new SocketClosedException();
                    }

                    throw new InvalidOperationException(
                        "Cannot listen, because socket is not closed.");
                }

                if (!this.IsBound)
                {
                    this.Bind();
                }

                this.State = SocketState.Listening;
                this.ReceiveBufferSize = receiveBuffer;
            }
        }

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

            if (count > this.LocalMiu)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(count),
                    "Message size exceeds send maximum information size.");
            }

            if (this.State != SocketState.Established)
            {
                if (this.State == SocketState.Closing)
                {
                    throw new BrokenPipeException();
                }

                throw new NotConnectedException();
            }

            // initialize handle to wait for.
            using (var handle = new ManualResetEventSlim(false))
            {
                var pendingSend = new PendingSend
                {
                    ManualResetEvent = handle
                };

                var buffer = new byte[count];
                Array.Copy(message, offset, buffer, 0, count);

                // Make sure the item and pendingSend are put at the same order.
                lock (this.sendLock)
                {
                    this.sendQueue.Enqueue(buffer);
                    this.pendingSendQueue.Enqueue(pendingSend);
                }

                // Wait for just created the send handle to notify.
                // May throw operation cancelled, in which case the message is
                // still enqueued... Maybe fix that later.
                // TODO: Remove cancelled items from the send queue.
                handle.Wait(token);

                if (!pendingSend.Success)
                {
                    // TODO: Throw more specific exception.
                    throw new CommunicationException(
                        pendingSend.Message,
                        pendingSend.InnerException);
                }
            }
        }

        public byte[] Receive() => this.Receive(new CancellationToken(false));

        public byte[] Receive(CancellationToken token)
        {
            // TODO: Make sure sap is always connected when bound.
            if (!this.IsBound)
            {
                throw new NotBoundException();
            }

            // This makes sure clients fall through one by one.
            this.receiveSemaphore.Wait(token);

            try
            {
                // This makes sure a message is available.
                this.receiveAvailable.Wait(token);

                // TODO: Make sure this is an information unit.
                if (!this.receiveQueue.TryDequeue(out ProtocolDataUnit result))
                {
                    // TODO: Log a horrible bug has occurred, or the socket was
                    // closed.
                    throw new OperationCanceledException(
                        "Socket was closed during operation.");
                }

                // Make sure the count check and the reset happen in a single go.
                lock (this.receiveLock)
                {
                    if (this.receiveQueue.Count == 0)
                    {
                        this.receiveAvailable.Reset();
                    }

                    this.pendingReceiveConfirmations--;
                    this.receiveState++;
                }

                return result.Information;
            }
            finally
            {
                // make space for the next receive
                this.receiveSemaphore.Release();
            }
        }

        /// <summary>
        /// Enqueues a received pdu to the socket pipeline.
        /// </summary>
        /// <remarks>This method is called by the <see cref="llc"/> thread. The
        /// received pdu is processed and determines whether the pdu is put to
        /// the receive queue, or a response is needed on the send queue.
        /// </remarks>
        /// <param name="pdu">Pdu.</param>
        public void EnqueueReceived(ProtocolDataUnit pdu)
        {
            lock (this.sendLock)
            lock (this.receiveLock)
            {
                if (!this.DetermineConnectionModeUnit(pdu))
                {
                    return;
                }

                if (this.State == SocketState.Closed)
                {
                    var dm = new DisconnectedModeUnit(
                        pdu.DataLink.Reverse(),
                        DisconnectReason.NoActiveConnection);
                    this.sendFastLane.Enqueue(dm);
                    return;
                }

                if (this.State == SocketState.Listening
                    && pdu.Type == ProtocolDataUnitType.Connect)
                {
                    // Remote client requests connection to local server
                    if (this.receiveQueue.Count < this.ReceiveBufferSize)
                    {
                        this.receiveQueue.Enqueue(pdu);
                        this.receiveAvailable.Set();
                    }
                    else
                    {
                        // Refuse remote client due to full receive buffer.
                        var dm = new DisconnectedModeUnit(
                            pdu.DataLink.Reverse(),
                            DisconnectReason.TemporaryInvalidAddress);
                        this.sendFastLane.Enqueue(dm);
                    }

                    return;
                }

                if (this.State == SocketState.Connecting
                    && (pdu.Type == ProtocolDataUnitType.ConnectionComplete
                        || pdu.Type == ProtocolDataUnitType.DisconnectedMode))
                {
                    // This pdu is for the Connect() method.
                    this.receiveQueue.Enqueue(pdu);
                    this.receiveAvailable.Set();
                    return;
                }

                if (this.State == SocketState.Disconnecting
                    && pdu.Type == ProtocolDataUnitType.DisconnectedMode)
                {
                    // TODO: Is ignoring this allright?
                    return;
                }

                if (this.State != SocketState.Established)
                {
                    // Can't provide a suitable response for the received pdu.
                    return;
                }

                if (pdu.Type == ProtocolDataUnitType.Information)
                {
                    FrameRejectUnit reject = null;
                    if (pdu.Information?.Length > this.LocalMiu)
                    {
                        // Message too large
                        reject = new FrameRejectUnit(
                            pdu.DataLink.Reverse(),
                            pdu.Type,
                            this.CurrentStateVariable,
                            this.CurrentAckStateVariable,
                            pdu.Sequence,
                            incorrectInformation: true);
                    }
                    else if (pdu.Sequence.Value.SendSequence 
                        != (this.receiveState + this.pendingReceiveConfirmations))
                    {
                        // Unexpected send sequence number
                        reject = new FrameRejectUnit(
                            pdu.DataLink.Reverse(),
                            pdu.Type,
                            this.CurrentStateVariable,
                            this.CurrentAckStateVariable,
                            pdu.Sequence,
                            invalidSendSequence: true);
                    }

                    if (reject != null)
                    {
                        this.sendFastLane.Enqueue(reject);
                        return;
                    }
                }

                if (pdu.Type == ProtocolDataUnitType.FrameReject)
                {
                    this.State = SocketState.Shutdown;
                    this.Close();
                    return;
                }

                if (pdu.Type == ProtocolDataUnitType.Disconnect)
                {
                    this.State = SocketState.Closing;
                    this.ClearSendQueues();
                    var dm = new DisconnectedModeUnit(
                        pdu.DataLink.Reverse(),
                        DisconnectReason.DisconnectReceived);
                    this.sendFastLane.Enqueue(dm);
                    return;
                }

                if (pdu.Type == ProtocolDataUnitType.ReceiveNotReady)
                {
                    this.sendAckState = pdu.Sequence.Value.ReceiveSequence;
                    this.sendBusy = true;
                    return;
                }

                if (pdu.Type == ProtocolDataUnitType.ReceiveReady)
                {
                    this.sendAckState = pdu.Sequence.Value.ReceiveSequence;
                    this.sendBusy = false;
                    return;
                }

                if (pdu.Type == ProtocolDataUnitType.Information)
                {
                    this.sendAckState = pdu.Sequence.Value.ReceiveSequence;
                    this.pendingReceiveConfirmations++;

                    if (this.receiveQueue.Count < this.ReceiveBufferSize)
                    {
                        this.receiveQueue.Enqueue(pdu);
                        this.receiveAvailable.Set();
                    }
                    else
                    {
                        // TODO: LOG warning information unit was discarded.
                    }

                    return;
                }
            }
        }

        private void ClearClientSendQueue()
        {
            lock (this.sendLock)
            lock(this.receiveLock)
            {
                while (this.sendQueue.TryDequeue(out byte[] result))
                {
                    // discard.
                }

                while (this.pendingSendQueue.TryDequeue(out PendingSend ps))
                {
                    ps.Success = false;
                    ps.Message = "Socket closed/closing.";
                    ps.ManualResetEvent.Set();
                }
            }
        }

        private void ClearSendQueues()
        {
            lock (this.sendLock)
            lock (this.receiveLock)
            {
                this.ClearClientSendQueue();

                while (this.sendFastLane.TryDequeue(out ProtocolDataUnit pdu))
                {
                    // discard
                }
            }
        }   

        private bool DetermineConnectionModeUnit(ProtocolDataUnit pdu)
        {
            switch (pdu.Type)
            {
                case ProtocolDataUnitType.Connect:
                case ProtocolDataUnitType.Disconnect:
                case ProtocolDataUnitType.ConnectionComplete:
                case ProtocolDataUnitType.DisconnectedMode:
                case ProtocolDataUnitType.FrameReject:
                case ProtocolDataUnitType.Information:
                case ProtocolDataUnitType.ReceiveReady:
                case ProtocolDataUnitType.ReceiveNotReady:
                    return true;
                default:
                    // This is not a connection mode pdu.
                    var reject = new FrameRejectUnit(
                        pdu.DataLink.Reverse(),
                        pdu.Type,
                        this.CurrentStateVariable,
                        this.CurrentAckStateVariable,
                        pdu.Sequence,
                        malformed: true);
                    this.sendFastLane.Enqueue(reject);
                    this.Close();
                    return false;
            }
        }

        public ProtocolDataUnit DequeueForSend(int maxInformationUnit)
        {
            lock (this.sendLock)
            {
                if (this.sendFastLane.TryPeek(out ProtocolDataUnit pdu))
                {
                    if (pdu.Information == null || 
                        pdu.Information.Length < maxInformationUnit)
                    {
                        this.sendFastLane.TryDequeue(out pdu);
                        return pdu;
                    }
                }

                // make sure the remote receive window is not exceeded.
                var diff = this.sendState - this.sendAckState;
                if (diff >= this.RemoteReceiveWindowSize)
                {
                    return null;
                }

                if (this.sendQueue.TryPeek(out byte[] buffer)
                    && buffer.Length <= maxInformationUnit)
                {
                    this.sendQueue.TryDequeue(out buffer);
                }
                else
                {
                    return null;
                }

                var link = new DataLink(
                    this.Address.Value, 
                    this.Peer.Value);
                return new InformationUnit(
                    link, 
                    this.CurrentStateVariable, 
                    buffer);
            }
        }

        /// <summary>
        /// Called by the <see cref="llc"/> to indicate a pdu has been sent
        /// successfully or not.
        /// </summary>
        /// <remarks>This method will be called in the order the pdus have been
        /// dequeued by the <see cref="DequeueForSend(Int32)"/> method, and 
        /// directly after transmission. The transmission may be an aggregated
        /// frame unit. In that case this method will be called multiple times 
        /// in a row, for each pdu sent (or not sent).
        /// </remarks>
        /// <param name="pdu">The .</param>
        /// <param name="success">If set to <c>true</c> success.</param>
        /// <param name="message">Message.</param>
        /// <param name="inner">Inner.</param>
        public void SendNotification(
            ProtocolDataUnit pdu,
            bool success,
            string message,
            Exception inner)
        {
            bool isInformation = false;
            bool isConnect = false;
            switch (pdu.Type)
            {
                case ProtocolDataUnitType.Connect:
                    isConnect = true;
                    break;
                case ProtocolDataUnitType.Information:
                    isInformation = true;
                    break;
                case ProtocolDataUnitType.ReceiveReady:
                case ProtocolDataUnitType.ReceiveNotReady:
                    break;
                default:
                    return;
            }

            // This was a I, RR or RNR unit, so contains sequence numbers for
            // acknowledgement.

            lock (this.sendLock)
            {
                if (!isConnect && success)
                {
                    // update the receive ack state.
                    var oldState = this.receiveAckState;
                    var newState = pdu.Sequence.Value.ReceiveSequence;
                    var diff = newState - oldState;
                    this.receiveAckState = newState;
                    lock (this.receiveLock)
                    {
                        // a few less pending receive confirmations.
                        this.pendingReceiveConfirmations -= diff;
                    }
                }
                else if (isInformation && !success)
                {
                    // If an information unit was not successfully transmitted,
                    // decrement the sendstate, in order to reuse it.
                    this.sendState--;
                }
            }

            if (isInformation || isConnect)
            {
                if (!this.pendingSendQueue.TryDequeue(out PendingSend result))
                {
                    // TODO: Log the terrible case that a send confirmation has
                    // not come through. This may have happened because the 
                    // socket was closed while transmitting. (so the pending
                    // send queue was cleared)
                }

                result.Success = success;
                result.Message = message;
                result.InnerException = inner;
                result.ManualResetEvent.Set();
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
                lock (this.sendLock)
                lock (this.receiveLock)
                {
                    this.State = SocketState.Closing;
                    
                    // Clear any pending sends and make sure waiting clients
                    // are released.
                    this.ClearClientSendQueue();

                    // Make sure clients waiting in the Receive() method pass
                    // through.
                    this.receiveAvailable.Set();

                    // TODO: notify llc the socket has been closed.
                    this.llc = null;

                    // NOTE: Queues will not be cleared, because it may be 
                    // unnecessary to do so.
                    this.State = SocketState.Shutdown;
                }
            }
        }
    }
}

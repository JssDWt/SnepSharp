//
//  LogicalLinkControl.cs
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading;
    using SnepSharp.Llcp.Parameters;
    using SnepSharp.Llcp.Pdus;
    using SnepSharp.Mac;

    /// <summary>
    /// A Logical Link Control (LLC)
    /// </summary>
    public class LogicalLinkControl
    {
        private static readonly Regex serviceNameFormat = new Regex(
            @"^urn:nfc:[x]?sn:[a-zA-Z][a-zA-Z0-9-_:\.]*$", 
            RegexOptions.Compiled);

        private static readonly Dictionary<string, byte> wellKnownServiceMap =
            new Dictionary<string, byte>{
                { "urn:nfc:sn:sdp", 1 },
                { "urn:nfc:sn:snep", 4 },
            };
        private object padlock = new object();
        internal ServiceAccessPoint[] ServiceAccessPoints { get; }
        private Dictionary<string, byte> serviceNames 
            = new Dictionary<string, byte>();
        private MacMapping mac;
        private PduCounter counter = new PduCounter();
        private ServiceDiscoveryPoint serviceDiscoveryPoint;

        public LinkServiceClass LinkServiceClass { get; } 
            = LinkServiceClass.ConnectionOriented;
        public bool IsInitiator { get; private set; }
        public SocketState State { get; private set; } = SocketState.Shutdown;
        public int ReceiveMiu { get; }
        public int SendLinkTimeout { get; }

        public int SendMiu { get; private set; }
        public int ReceiveLinkTimeout { get; private set; }
        public ICollection<LinkAddress> RemoteWks { get; private set; }
        public LlcpVersion RemoteVersion { get; private set; }

        public LogicalLinkControl(LlcpOptions options)
        {
            if (options == null)
            {
                options = new LlcpOptions();
            }

            this.ReceiveMiu = options.MaximumInformationUnit;
            this.SendLinkTimeout = (int)options.LinkTimeout.TotalMilliseconds;
            this.serviceNames.Add(
                Constants.ServiceDiscoveryProtocolName, 
                1);
            this.ServiceAccessPoints = new ServiceAccessPoint[0x40];
            this.ServiceAccessPoints[0] = new ServiceAccessPoint(
                (LinkAddress)0, 
                this);
            this.serviceDiscoveryPoint = new ServiceDiscoveryPoint(this);
            this.ServiceAccessPoints[1] = this.serviceDiscoveryPoint;
        }

        public DataLinkConnection CreateSocket() => new DataLinkConnection(this);

        public ServerSocket CreateServerSocket() => new ServerSocket(this);

        /// <summary>
        /// Activates the LLCP connection.
        /// </summary>
        /// <param name="mac">Mac layer mapping.</param>
        internal bool Activate(MacMapping mac)
        {
            var sendPax = this.CreatePax();
            this.IsInitiator = mac.IsInitiator;
            this.State = this.IsInitiator 
                ? SocketState.Connecting 
                : SocketState.Listening;

            // TODO: catch and return false.
            ParameterExchangeUnit receivePax = mac.Activate(sendPax);
            if (!this.IsInitiator && mac.ResponseWaitingTime 
                >= this.SendLinkTimeout)
            {
                // TODO: Determine what to do when rwt is greater than lto.
            }

            this.SetRemoteParameters(receivePax);
            this.mac = mac;
            this.State = SocketState.Connected;
            return true;
        }

        /// <summary>
        /// Terminates the LLCP connection.
        /// </summary>
        internal void Terminate()
        {
            if (this.IsInitiator && this.State == SocketState.Disconnecting)
            {
                this.Exchange(
                    new DisconnectUnit(DataLink.Empty),
                    500);
            }

            this.mac.Deactivate();
            for (int i = 2; i < this.ServiceAccessPoints.Length; i++)
            {
                var sap = this.ServiceAccessPoints[i];
                this.ServiceAccessPoints[i] = null;
                sap.Close();
            }

            this.State = SocketState.Shutdown;
        }

        internal void Bind(DataLinkConnection socket, string serviceName)
        {
            if (socket == null) throw new ArgumentNullException(nameof(socket));
            if (serviceName == null)
            {
                throw new ArgumentNullException(nameof(serviceName));
            }

            if (!serviceNameFormat.IsMatch(serviceName))
            {
                throw new ArgumentException(
                    "Invalid service name.", 
                    nameof(serviceName));
            }

            lock (this.padlock)
            {
                if (this.serviceNames.Keys.Contains(serviceName))
                {
                    throw new InvalidOperationException("Service name in use.");
                }

                LinkAddress address;
                if (wellKnownServiceMap.ContainsKey(serviceName))
                {
                    address = (LinkAddress)wellKnownServiceMap[serviceName];
                }
                else
                {
                    address = this.GetAvailableAddress();
                }

                this.ServiceAccessPoints[address] 
                    = new ServiceAccessPoint(address, this);
                this.ServiceAccessPoints[address].AddSocket(socket);
                this.serviceNames.Add(serviceName, address);
            }
        }

        internal void Bind(DataLinkConnection socket, LinkAddress address)
        {
            if (socket == null) throw new ArgumentNullException(nameof(socket));
            if (!LinkAddress.UpperLayerSaps.Contains(address))
            {
                throw new InvalidOperationException(
                    "Must bind to upper layer SAP addresses only.");
            }

            lock (this.padlock)
            {
                if (this.ServiceAccessPoints[address] != null)
                {
                    throw new InvalidOperationException("Address in use.");
                }

                this.ServiceAccessPoints[address] 
                    = new ServiceAccessPoint(address, this);

                this.ServiceAccessPoints[address].AddSocket(socket);
            }
        }

        internal void Bind(DataLinkConnection socket)
        {
            if (socket == null) throw new ArgumentNullException(nameof(socket));

            lock (this.padlock)
            {
                LinkAddress address = this.GetAvailableAddress();

                this.ServiceAccessPoints[address] 
                    = new ServiceAccessPoint(address, this);
                this.ServiceAccessPoints[address].AddSocket(socket);
            }
        }

        internal void AddToSap(DataLinkConnection socket)
        {
            if (socket == null) throw new ArgumentNullException(nameof(socket));
            lock (this.padlock)
            {
                var sap = this.ServiceAccessPoints[socket.Address.Value];
                if (sap == null)
                {
                    throw new InvalidOperationException(
                        "Cannot add socket to unbound service access point.");
                }

                sap.AddSocket(socket);
            }
        }

        internal bool Send(DataLinkConnection socket, ProtocolDataUnit message)
        {
            throw new NotImplementedException();
        }

        internal bool SendTo(
            DataLinkConnection socket, 
            ProtocolDataUnit message, 
            LinkAddress destination)
        {
            throw new NotImplementedException();
        }

        internal ProtocolDataUnit Receive(DataLinkConnection socket)
        {
            throw new NotImplementedException();
        }

        private LinkAddress GetAvailableAddress()
        {
            LinkAddress? address = LinkAddress.UpperLayerSaps
                    .FirstOrDefault(a => this.ServiceAccessPoints[a] == null);

            if (!address.HasValue)
            {
                throw new InvalidOperationException(
                    "All upper layer service access point addresses are " +
                    "taken.");
            }

            return address.Value;
        }

        /// <summary>
        /// Disconnects from the client, then terminates.
        /// </summary>
        private void Disconnect()
        {
            this.State = SocketState.Disconnecting;
            this.Terminate();
        }

        /// <summary>
        /// Exchange the specified sendPdu with the mac layer, respecting the
        /// specified timeout.
        /// </summary>
        /// <returns>The response pdu, or null if no response is available.
        /// </returns>
        /// <param name="sendPdu">pdu to send.</param>
        /// <param name="timeout">Timeout.</param>
        private ProtocolDataUnit Exchange(
            ProtocolDataUnit sendPdu, 
            int timeout)
        {
            if (sendPdu != null)
            {
                this.counter.Sent[sendPdu.Type] += 1;
            }

            // TODO: catch?
            var receivePdu = this.mac.Exchange(sendPdu, timeout);

            if (receivePdu != null)
            {
                this.counter.Received[receivePdu.Type] += 1;
            }

            return receivePdu;
        }


        /// <summary>
        /// Creates the parameter exchange pdu.
        /// </summary>
        /// <returns>The pax.</returns>
        private ParameterExchangeUnit CreatePax()
        {
            var boundWks = this.serviceNames.Values.Cast<LinkAddress>()
                .Intersect(LinkAddress.WellknownSaps)
                .ToList();

            // TODO: If connectionless transport is supported, add it here.
            var pax = new ParameterExchangeUnit(
                LlcpVersion.V13,
                this.ReceiveMiu,
                boundWks,
                this.SendLinkTimeout,
                LinkServiceClass.ConnectionOriented);

            return pax;
        }

        /// <summary>
        /// Sets the remote parameters.
        /// </summary>
        /// <param name="receivePax">Receive pax.</param>
        private void SetRemoteParameters(ParameterExchangeUnit receivePax)
        {
            var version = (VersionParameter)receivePax.Parameters
                .First(p => p.Type == ParameterType.Version);
            this.RemoteVersion = version.Version;

            var remoteMiux = (MiuxParameter)receivePax.Parameters
                .FirstOrDefault(p => p.Type == ParameterType.MiuxExtension);
            this.SendMiu = remoteMiux?.MaximumInformationUnit
                ?? Constants.MaximumInformationUnit;

            var wks = (WksParameter)receivePax.Parameters.FirstOrDefault(
                p => p.Type == ParameterType.WellKnownServiceList);
            this.RemoteWks = wks?.BoundAddresses ?? new List<LinkAddress>();

            var lto = (LinkTimeoutParameter)receivePax.Parameters
                .FirstOrDefault(p => p.Type == ParameterType.LinkTimeout);
            int timeoutMs = lto?.TimeoutMilliseconds
                ?? Constants.DefaultTimeout;
            this.ReceiveLinkTimeout = timeoutMs;
        }

        /// <summary>
        /// Runs the logical link control.
        /// </summary>
        /// <param name="token">Token.</param>
        public void Run(CancellationToken token)
        {
            if (this.IsInitiator)
            {
                this.RunAsInitiator(token);
            }
            else
            {
                this.RunAsTarget(token);
            }
        }

        /// <summary>
        /// Runs the <see cref="LogicalLinkControl"/> as target.
        /// </summary>
        /// <param name="token">Cancellation token.</param>
        private void RunAsTarget(CancellationToken token)
        {
            // TODO: is this +10 necessary?
            var receiveTimeout = this.ReceiveLinkTimeout + 10;

            int consecutiveSymms = 0;
            var receivePdu = this.Exchange(null, receiveTimeout);
            this.State = SocketState.Established;

            while (!token.IsCancellationRequested)
            {
                if (receivePdu == null)
                {
                    this.Terminate();
                    return;
                }

                if (receivePdu.Type == ProtocolDataUnitType.Disconnect)
                {
                    this.State = SocketState.Closed;
                    this.Terminate();
                    return;
                }

                if (receivePdu.Type == ProtocolDataUnitType.Symmetry)
                {
                    consecutiveSymms++;
                }
                else
                {
                    consecutiveSymms = 0;
                }

                this.Dispatch(receivePdu);
                var sendPdu = this.Collect(TimeSpan.FromMilliseconds(1));
                if (sendPdu == null && consecutiveSymms >= 10)
                {
                    sendPdu = this.Collect(TimeSpan.FromMilliseconds(50));
                }

                if (sendPdu == null)
                {
                    sendPdu = new SymmetryUnit();
                }

                receivePdu = this.Exchange(sendPdu, receiveTimeout);
            }

            this.Disconnect();
        }

        /// <summary>
        /// Runs the <see cref="LogicalLinkControl"/> as initiator.
        /// </summary>
        /// <param name="token">Cancelation token.</param>
        private void RunAsInitiator(CancellationToken token)
        {
            // TODO: Is this +10 milliseconds necessary?
            var receiveTimeout = this.ReceiveLinkTimeout + 10;

            int consecutiveSymms = 0;
            var sendPdu = this.Collect(TimeSpan.FromMilliseconds(100));
            this.State = SocketState.Established;

            while (!token.IsCancellationRequested)
            {
                if (sendPdu == null)
                {
                    sendPdu = new SymmetryUnit();
                }

                var receivePdu = this.Exchange(sendPdu, receiveTimeout);
                if (receivePdu == null)
                {
                    this.Terminate();
                    return;
                }

                if (receivePdu.Type == ProtocolDataUnitType.Disconnect)
                {
                    this.State = SocketState.Closed;
                    this.Terminate();
                    return;
                }

                if (receivePdu.Type == ProtocolDataUnitType.Symmetry)
                {
                    consecutiveSymms++;
                }
                else
                {
                    consecutiveSymms = 0;
                }

                sendPdu = this.Collect(TimeSpan.FromMilliseconds(1));
                if (sendPdu == null && consecutiveSymms >= 10)
                {
                    sendPdu = this.Collect(TimeSpan.FromMilliseconds(50));
                }
            }

            this.Disconnect();
        }

        /// <summary>
        /// Collect a protocol data unit available for sending from the service
        /// access points.
        /// </summary>
        /// <returns>The collected pdu, or null if none is available.</returns>
        /// <param name="delay">Delay defore collection.</param>
        private ProtocolDataUnit Collect(TimeSpan? delay)
        {
            if (delay.HasValue)
            {
                // TODO: Make this sleep Task.Delay?
                Thread.Sleep(delay.Value);
            }

            lock (this.padlock)
            {
                var activeSaps = this.ServiceAccessPoints.Where(
                    sap => sap != null).ToList();
                ProtocolDataUnit sendPdu = null;
                foreach (var sap in activeSaps)
                {
                    sendPdu = sap.SendAcknowledgement();
                    if (sendPdu != null)
                    {
                        return sendPdu;
                    }
                }

                foreach (var sap in activeSaps)
                {
                    sendPdu = sap.DequeueForSend(this.ReceiveMiu);
                    if (sendPdu != null)
                    {
                        return sendPdu;
                    }
                }

                // TODO: support aggregated frame pdu
            }

            return null;
        }

        /// <summary>
        /// Dispatch the specified receivePdu to the right service access point.
        /// </summary>
        /// <param name="receivePdu">Receive pdu to dispatch.</param>
        private void Dispatch(ProtocolDataUnit receivePdu)
        {
            if (receivePdu.Type == ProtocolDataUnitType.Symmetry)
            {
                return;
            }

            if (receivePdu.Type == ProtocolDataUnitType.AggregatedFrame)
            {
                if (receivePdu.DataLink == DataLink.Empty)
                {
                    var agg = (AggregatedFrameUnit)receivePdu;
                    foreach (var pdu in agg.Aggregate)
                    {
                        this.Dispatch(pdu);
                    }
                }

                return;
            }

            lock (this.padlock)
            {
                if (receivePdu.Type == ProtocolDataUnitType.Connect
                && receivePdu.DataLink.Destination 
                    == LinkAddress.ServiceDiscoveryProtocolSap)
                {
                    var connect = (ConnectUnit)receivePdu;
                    bool disconnect = false;
                    DisconnectReason reason 
                        = DisconnectReason.PermanentlyUnavailable;
                    byte address = 0;
                    if (connect.ServiceName == null)
                    {
                        disconnect = true;
                        reason = DisconnectReason.PermanentInvalidAddress;
                    }
                    else if (this.serviceNames.TryGetValue(
                        connect.ServiceName,
                        out address))
                    {
                        // service name bound
                        var s = this.ServiceAccessPoints[address];
                        if (s == null)
                        {
                            // service name bound, but no service available.
                            disconnect = true;
                            reason = DisconnectReason.NoServiceBound;
                        }
                    }
                    else
                    {
                        // serivce name not bound.
                        disconnect = true;
                        reason = DisconnectReason.NoServiceBound;
                    }

                    if (disconnect)
                    {
                        var disconnectPdu = new DisconnectedModeUnit(
                            new DataLink(
                                LinkAddress.ServiceDiscoveryProtocolSap,
                                receivePdu.DataLink.Source),
                            reason);
                        this.serviceDiscoveryPoint.DisconnectedModePdus.Add(
                            disconnectPdu);
                        return;
                    }

                    // service found. Rewrite the connect unit to the bound service.
                    receivePdu = new ConnectUnit(
                        new DataLink(
                            receivePdu.DataLink.Source,
                            (LinkAddress)address),
                        connect.MaximumInformationUnit,
                        connect.ReceiveWindowSize);
                }

                var sap = this.ServiceAccessPoints[
                    receivePdu.DataLink.Destination];
                if (sap != null)
                {
                    sap.EnqueueReceived(receivePdu);
                }
            }
        }
    }
}

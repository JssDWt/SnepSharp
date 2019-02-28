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
    using System.Threading;
    using SnepSharp.Llcp.Parameters;
    using SnepSharp.Llcp.Pdus;
    using SnepSharp.Mac;

    /// <summary>
    /// A Logical Link Control (LLC)
    /// </summary>
    public class LogicalLinkControl
    {
        private object padlock = new object();
        internal ServiceAccessPoint[] ServiceAccessPoints { get; }
        private Dictionary<string, byte> serviceNames 
            = new Dictionary<string, byte>();
        private MacMapping mac;
        private PduCounter counter = new PduCounter();

        public LinkServiceClass LinkServiceClass { get; } 
            = LinkServiceClass.ConnectionOriented;
        public bool IsInitiator { get; private set; }
        public SocketState State { get; private set; } = SocketState.Shutdown;
        public int ReceiveMiu { get; }
        public TimeSpan SendLinkTimeout { get; }

        public int SendMiu { get; private set; }
        public TimeSpan ReceiveLinkTimeout { get; private set; }
        public ICollection<LinkAddress> RemoteWks { get; private set; }
        public LlcpVersion RemoteVersion { get; private set; }

        public LogicalLinkControl(LlcpOptions options)
        {
            if (options == null)
            {
                options = new LlcpOptions();
            }

            this.ReceiveMiu = options.MaximumInformationUnit;
            this.SendLinkTimeout = options.LinkTimeout;
            this.serviceNames.Add(
                Constants.ServiceDiscoveryProtocolName, 
                1);
            this.ServiceAccessPoints = new ServiceAccessPoint[0x40];
            this.ServiceAccessPoints[0] = new ServiceAccessPoint(
                (LinkAddress)0, 
                this);
            this.ServiceAccessPoints[1] = new ServiceDiscoveryPoint(this);
        }

        public LlcpSocket CreateLlcpSocket()
        {
            throw new NotImplementedException();
        }

        public LlcpServerSocket CreateLlcpServerSocket()
        {
            throw new NotImplementedException();
        }

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
                    TimeSpan.FromMilliseconds(500));
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

        private void Disconnect()
        {
            this.State = SocketState.Disconnecting;
            this.Terminate();
        }

        internal ProtocolDataUnit Exchange(
            ProtocolDataUnit sendPdu, 
            TimeSpan timeout)
        {
            if (sendPdu != null)
            {
                this.counter.Sent[sendPdu.Type] += 1;
            }

            // TODO: catch?
            var receivePdu = this.mac.Exchange(sendPdu);

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
            var parameters = new ParameterList();

            // this.macMapping = null;
            var boundWks = this.serviceNames.Values.Cast<LinkAddress>()
                .Intersect(LinkAddress.WellknownSaps)
                .ToList();
            var wks = new WksParameter(boundWks);
            parameters.Add(wks);

            if (this.ReceiveMiu != Constants.MaximumInformationUnit)
            {
                parameters.Add(new MiuxParameter(
                    this.ReceiveMiu - Constants.MaximumInformationUnit));
            }

            if (this.SendLinkTimeout.Milliseconds != Constants.DefaultTimeout)
            {
                parameters.Add(new LinkTimeoutParameter(
                    this.SendLinkTimeout.Milliseconds));
            }

            // TODO: How to get this datalink??
            var zeroLink = new DataLink((LinkAddress)0, (LinkAddress)0);
            return new ParameterExchangeUnit(zeroLink, parameters);
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
            this.SendMiu = remoteMiux?.ActualMiu
                ?? Constants.MaximumInformationUnit;

            var wks = (WksParameter)receivePax.Parameters.FirstOrDefault(
                p => p.Type == ParameterType.WellKnownServiceList);
            this.RemoteWks = wks?.BoundAddresses ?? new List<LinkAddress>();

            var lto = (LinkTimeoutParameter)receivePax.Parameters
                .FirstOrDefault(p => p.Type == ParameterType.LinkTimeout);
            int timeoutMs = lto?.TimeoutMilliseconds
                ?? Constants.DefaultTimeout;
            this.ReceiveLinkTimeout = TimeSpan.FromMilliseconds(timeoutMs);
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

        private void RunAsTarget(CancellationToken token)
        {
            var receiveTimeout = this.ReceiveLinkTimeout
                + TimeSpan.FromSeconds(10);

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
                    sendPdu = new SymmetryUnit(DataLink.Empty);
                }

                receivePdu = this.Exchange(sendPdu, receiveTimeout);
            }

            this.State = SocketState.Disconnecting;
            this.Terminate();
        }

        private void RunAsInitiator(CancellationToken token)
        {
            var receiveTimeout = this.ReceiveLinkTimeout 
                + TimeSpan.FromSeconds(10);

            int consecutiveSymms = 0;
            var sendPdu = this.Collect(TimeSpan.FromMilliseconds(100));
            this.State = SocketState.Established;

            while (!token.IsCancellationRequested)
            {
                if (sendPdu == null)
                {
                    sendPdu = new SymmetryUnit(DataLink.Empty);
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

            this.State = SocketState.Disconnecting;
            this.Terminate();
        }

        private ProtocolDataUnit Collect(TimeSpan? delay)
        {
            if (delay.HasValue)
            {
                // TODO: Make this sleep Task.Delay?
                Thread.Sleep(delay.Value);
            }

            lock (this.padlock)
            {
                ProtocolDataUnit sendPdu = null;
                foreach (var sap in this.ServiceAccessPoints.Where(
                    sap => sap != null))
                {
                    sendPdu = sap.SendAcknowledgement();
                    if (sendPdu != null)
                    {
                        break;
                    }
                }

                if (sendPdu == null) return null;

                // TODO: support aggregated frame pdu
                return sendPdu;
            }
        }

        private void Dispatch(ProtocolDataUnit receivePdu)
        {
            throw new NotImplementedException();
        }
    }
}

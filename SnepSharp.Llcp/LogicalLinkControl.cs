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
    using SnepSharp.Llcp.Parameters;
    using SnepSharp.Llcp.Pdus;

    /// <summary>
    /// A Logical Link Control (LLC)
    /// </summary>
    public class LogicalLinkControl
    {
        public int MaximumInformationUnit { get; private set; } = Constants.MaximumInformationUnit;
        private int MaxUnacknowledgedUnits = 1;
        private HashSet<DataLink> connections;
        private Dictionary<DataLink, Dictionary<int, ProtocolDataUnit>> unacknowledgedUnits;

        public LogicalLinkControl()
        {
        }

        public static LogicalLinkControl GetInstance()
        {
            throw new NotImplementedException();
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
        /// Called when a peer device capable of executing LLCP enters 
        /// communication range and the MAC link activation procedure has been 
        /// successfully completed.
        /// </summary>
        /// <param name="mapping">Mapping.</param>
        private void OnPeerDeviceConnected(MacMapping mapping)
        {
            this.ActivateLink(mapping);
            // link MIU agreement, enter normal operation, otherwise abandon link and notify mac activation failed.
        }

        private void ActivateLink(MacMapping mapping)
        {
            var remotePax = this.ExchangeParameters(mapping);
            if (!this.AgreeVersion(remotePax))
            {
                NotifyLinkActivationFailed(mapping);
            }

            this.AgreeMiu();
            if (!mapping.IsInitiator)
            {
                var paxPdu = CreateParameters(mapping);
                Send(paxPdu);
            }
        }

        private ParameterExchangeUnit ExchangeParameters(MacMapping mapping)
        {
            if (!mapping.ParameterExchangeAllowed) return null;

            ProtocolDataUnit remotePax;
            if (mapping.IsInitiator)
            {
                // TODO: Figure out the other parameters to send. (only the ones not received in the mac mapping)
                var paxPdu = CreateParameters(mapping);
                remotePax = SendWithResponse(paxPdu);
            }
            else
            {
                remotePax = WaitForResponse();
            }

            if (remotePax.Type != ProtocolDataUnitType.ParameterExchange)
            {
                // TODO: What to do when we don't receive a parameter exchange? Terminate link?
            }

            // TODO: Handle the parameters
            return (ParameterExchangeUnit)remotePax;
        }

        private ParameterExchangeUnit CreateParameters(MacMapping mapping)
        {
            var parameters = new ParameterList
                {
                    new VersionParameter(LlcpVersion.V10)
                };
            var paxPdu = new ParameterExchangeUnit(new DataLink((LinkAddress)0, (LinkAddress)0), parameters);
            return paxPdu;
        }

        private bool AgreeVersion(ParameterExchangeUnit remotePax)
        {
            var version = (VersionParameter)remotePax.Parameters.Find(p => p.Type == ParameterType.Version);
            return version.Version.Major == LlcpVersion.V10.Major;
        }

        private bool AgreeMiu()
        {
            throw new NotImplementedException();
        }

        private void NotifyLinkActivationFailed(MacMapping mapping)
        {
            throw new NotImplementedException();
        }

        private void Send(ProtocolDataUnit pdu)
        {
            throw new NotImplementedException();
        }

        private ProtocolDataUnit SendWithResponse(ProtocolDataUnit pdu)
        {
            this.Send(pdu);
            return this.WaitForResponse();
        }

        private ProtocolDataUnit WaitForResponse()
        {
            throw new NotImplementedException();
        }

        private void Close(DataLink link)
        {
            // TODO Send close to remote llc

            this.unacknowledgedUnits.Remove(link);
            this.connections.Remove(link);
        }
    }
}

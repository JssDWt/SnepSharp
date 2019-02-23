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

        public void Start()
        {

        }

        public int Receive(byte[] buffer)
        {

        }

        public void Send(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            this.Send(buffer, buffer.Length);
        }

        public void Send(byte[] buffer, int length)
        {

        }

        public void Close()
        {

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
                var paxPdu = CreateParameters();
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
                var paxPdu = CreateParameters();
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
            var paxPdu = new ParameterExchangeUnit(new DataLink(0, 0), parameters);
            return paxPdu;
        }

        private bool AgreeVersion(ParameterExchangeUnit remotePax)
        {
            var version = (VersionParameter)remotePax.Parameters.Find(p => p.Type == ParameterType.Version);
            return version.Version.Major == LlcpVersion.V10.Major;
        }

        private bool AgreeMiu()
        {

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

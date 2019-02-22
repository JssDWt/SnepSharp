namespace SnepSharp.Llcp.Pdus
{
    using System;

    /// <summary>
    /// Information (I) unit. Is used to transfer service data units across a 
    /// data link connection.
    /// </summary>
    internal class InformationUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.InformationUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="sequence">Sequence number. Send part indicates the sequence 
        /// number for this pdu. Receive number indicates all PDUs up through that
        /// number have been received by the service layer.</param>
        /// <param name="serviceData">Service data.</param>
        public InformationUnit(DataLink connection, SequenceNumber sequence, byte[] serviceData)
            : base(connection, ProtocolDataUnitType.Information, sequence, serviceData)
        {
        }
    }
}

namespace Llcp
{
    using System;

    /// <summary>
    /// Receive ready (RR) unit. is used by an LLC to acknowledge one or more 
    /// received I PDUs and indicate that the LLC is able to receive subsequent 
    /// I PDUs.
    /// </summary>
    internal class ReceiveReadyUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.ReceiveReadyUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="sequence">Receive sequence number. Indicates all I pdus 
        /// up through that number have been processed and can be considered 
        /// acknowledged.</param>
        public ReceiveReadyUnit(DataLink connection, SequenceNumber sequence)
            : base(connection, ProtocolDataUnitType.ReceiveReady, sequence, null)
        {
        }
    }
}

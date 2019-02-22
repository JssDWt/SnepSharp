using System;
namespace Llcp
{
    /// <summary>
    /// Receive not ready (RNR) unit. Is used by an LLC to indicate a temporary 
    /// inability to process subsequent I PDUs.
    /// </summary>
    internal class ReceiveNotReadyUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.ReceiveNotReadyUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="sequence">Receive sequence number. Indicates all I pdus 
        /// up through that number have been processed and can be considered 
        /// acknowledged.</param>
        public ReceiveNotReadyUnit(DataLink connection, SequenceNumber sequence)
            : base(connection, ProtocolDataUnitType.ReceiveNotReady, sequence, null)
        {
        }
    }
}

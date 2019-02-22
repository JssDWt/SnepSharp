namespace SnepSharp.Llcp.Pdus
{
    using System;

    /// <summary>
    /// Disconnected mode (DM) unit. Is used to report status indicating that 
    /// the LLC is logically disconnected from the data link connection 
    /// identified by the DSAP and SSAP address pair.
    /// </summary>
    internal class DisconnectedModeUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.DisconnectedModeUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="reason">Disconnect reason.</param>
        public DisconnectedModeUnit(DataLink connection, DisconnectReason reason)
            : base(connection, ProtocolDataUnitType.DisconnectedMode, null, new byte[] { (byte)reason })
        {
        }
    }
}

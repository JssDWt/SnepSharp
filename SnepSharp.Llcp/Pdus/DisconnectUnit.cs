namespace SnepSharp.Llcp.Pdus
{
    /// <summary>
    /// Disconnect (DISC) unit. Is used to terminate a data link connection or 
    /// is used to deactivate the LLCP Link.
    /// </summary>
    internal class DisconnectUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.DisconnectUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        public DisconnectUnit(DataLink connection)
            : base(connection, ProtocolDataUnitType.Disconnect, null, null)
        {
        }
    }
}

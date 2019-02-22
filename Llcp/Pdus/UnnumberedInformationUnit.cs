namespace Llcp
{
    using System;

    /// <summary>
    /// Unnumbered information (UI) unit. Is used to transfer service data units
    /// to the peer LLC without prior establishment of a data link connection.
    /// </summary>
    internal class UnnumberedInformationUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.UnnumberedInformationUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="serviceData">Optional service data.</param>
        public UnnumberedInformationUnit(DataLink connection, byte[] serviceData)
            : base(connection, ProtocolDataUnitType.UnnumberedInformation, null, serviceData)
        {
        }
    }
}

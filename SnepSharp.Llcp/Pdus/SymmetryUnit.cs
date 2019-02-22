namespace SnepSharp.Llcp.Pdus
{
    /// <summary>
    /// Symmetry unit (SYMM). Sent by an LLC whenever no other PDUs are 
    /// available for sending, to ensure symmetry.
    /// </summary>
    internal class SymmetryUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.SymmetryUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        public SymmetryUnit(DataLink connection)
            : base(connection, ProtocolDataUnitType.Symmetry, null, null)
        {
        }
    }
}

namespace SnepSharp.Llcp.Pdus
{
    using SnepSharp.Llcp.Parameters;

    /// <summary>
    /// Connection complete (CC) unit. is used by an LLC to acknowledge the 
    /// receipt and acceptance of the CONNECT.
    /// </summary>
    internal class ConnectionCompleteUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.ConnectionCompleteUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="parameters">Optional connection specific parameters.</param>
        public ConnectionCompleteUnit(DataLink connection, ParameterList parameters)
            : base(connection, ProtocolDataUnitType.ConnectionComplete, null, ToBytes(parameters))
        {
        }

        /// <summary>
        /// Converts the parameters to bytes.
        /// </summary>
        /// <returns>The bytes, or null if the parameter list is null.</returns>
        /// <param name="parameters">Parameters to convert.</param>
        private static byte[] ToBytes(ParameterList parameters)
        {
            if (parameters == null) return null;
            return parameters.ToBytes();
        }

        /// <summary>
        /// Converts the specified bytes into a parameter list.
        /// </summary>
        /// <returns>Parameter list, or null if no parameters were supplied.</returns>
        /// <param name="bytes">Bytes to parse.</param>
        private static ParameterList FromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            return ParameterList.FromBytes(bytes);
        }
    }
}

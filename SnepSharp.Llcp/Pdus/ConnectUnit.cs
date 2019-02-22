namespace SnepSharp.Llcp.Pdus
{
    using SnepSharp.Llcp.Parameters;

    /// <summary>
    /// Connect (CONNECT) unit. Is used to request a data link connection 
    /// between a source and a destination service access point.
    /// </summary>
    internal class ConnectUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.ConnectUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="parameters">Optional connection specific parameters.</param>
        public ConnectUnit(DataLink connection, ParameterList parameters)
            : base(connection, ProtocolDataUnitType.Connect, null, ToBytes(parameters))
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

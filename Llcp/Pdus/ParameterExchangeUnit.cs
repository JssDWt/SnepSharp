namespace Llcp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Parameter exchange (PAX) unit. SHALL be used to exchange parameters 
    /// concerning the LLCP Link configuration.
    /// </summary>
    internal class ParameterExchangeUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public ParameterList Parameters { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.ParameterExchangeUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="parameters">Parameters.</param>
        public ParameterExchangeUnit(DataLink connection, ParameterList parameters)
            : base(connection, ProtocolDataUnitType.ParameterExchange, null, ToBytes(parameters))
        {
            this.Parameters = parameters;
        }

        /// <summary>
        /// Converts the specified parameters to bytes.
        /// </summary>
        /// <returns>The byte representation of the parameters.</returns>
        /// <param name="parameters">Parameters.</param>
        private static byte[] ToBytes(ParameterList parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                throw new ArgumentException("At least one parameter should be supplied.", nameof(parameters));
            }

            return parameters.ToBytes();
        }

        /// <summary>
        /// Parses the specified bytes into parameters.
        /// </summary>
        /// <returns>The parameters.</returns>
        /// <param name="data">bytes to parse.</param>
        private static ParameterList FromBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var list = ParameterList.FromBytes(data);
            if (list.Count == 0)
            {
                throw new ArgumentException("At least one parameter should be supplied.", nameof(data));
            }

            return list;
        }
    }
}

//
//  ParameterExchangeUnit.cs
//
//  Author:
//       Jesse de Wit <witdejesse@hotmail.com>
//
//  Copyright (c) 2019 
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

namespace SnepSharp.Llcp.Pdus
{
    using System;
    using SnepSharp.Llcp.Parameters;

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
        /// Initializes a new instance of the 
        /// <see cref="ParameterExchangeUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="parameters">Parameters.</param>
        public ParameterExchangeUnit(
            DataLink connection, 
            ParameterList parameters)
            : base(
                  connection, 
                  ProtocolDataUnitType.ParameterExchange, 
                  null, 
                  ToBytes(parameters))
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
                throw new ArgumentException(
                    "At least one parameter should be supplied.", 
                    nameof(parameters));
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
                throw new ArgumentException(
                    "At least one parameter should be supplied.", 
                    nameof(data));
            }

            return list;
        }
    }
}

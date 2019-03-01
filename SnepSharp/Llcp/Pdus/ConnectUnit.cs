//
//  ConnectUnit.cs
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
    using SnepSharp.Llcp.Parameters;

    /// <summary>
    /// Connect (CONNECT) unit. Is used to request a data link connection 
    /// between a source and a destination service access point.
    /// </summary>
    internal class ConnectUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Gets the maximum information unit extension. (MIUX)
        /// </summary>
        /// <value>The maximum information unit extension.</value>
        public int MaximumInformationUnitExtension { get; }

        /// <summary>
        /// Gets the size of the receive window. (RW)
        /// </summary>
        /// <value>The size of the receive window.</value>
        public int ReceiveWindowSize { get; }

        /// <summary>
        /// Gets the name of the service. (SN)
        /// </summary>
        /// <value>The name of the service.</value>
        public string ServiceName { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="miux">The maximum information unit extension.</param>
        /// <param name="receiveWindowSize">Receive window size.</param>
        /// <param name="serviceName">Service name.</param>
        public ConnectUnit(
            DataLink connection, 
            int miux = 0,
            int receiveWindowSize = 1,
            string serviceName = null)
            : base(
                connection, 
                ProtocolDataUnitType.Connect, 
                null, 
                ToBytes(miux, receiveWindowSize, serviceName))
        {
            this.MaximumInformationUnitExtension = miux;
            this.ReceiveWindowSize = receiveWindowSize;
            this.ServiceName = serviceName;
        }

        /// <summary>
        /// Converts the parameters to bytes.
        /// </summary>
        /// <returns>The bytes, or null if the parameter list is null.</returns>
        private static byte[] ToBytes(int miux, int rw, string sn)
        {
            var parameters = new ParameterList();
            if (miux != 0)
            {
                parameters.Add(new MiuxParameter(miux));
            }

            if (rw != 1)
            {
                parameters.Add(new ReceiveWindowSizeParameter(rw));
            }

            if (sn != null)
            {
                parameters.Add(new ServiceNameParameter(sn));
            }

            if (parameters.Count == 0) return null;
            return parameters.ToBytes();
        }

        /// <summary>
        /// Converts the specified bytes into a parameter list.
        /// </summary>
        /// <returns>Parameter list, or null if no parameters were supplied.
        /// </returns>
        /// <param name="bytes">Bytes to parse.</param>
        private static ParameterList FromBytes(byte[] bytes)
        {
            if (bytes == null || bytes.Length == 0) return null;
            return ParameterList.FromBytes(bytes);
        }
    }
}

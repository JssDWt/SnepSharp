//
//  ConnectionCompleteUnit.cs
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
    /// Connection complete (CC) unit. is used by an LLC to acknowledge the 
    /// receipt and acceptance of the CONNECT.
    /// </summary>
    internal class ConnectionCompleteUnit : ProtocolDataUnit
    {
        public int MaximumInformationUnit { get; }
        public int ReceiveWindowSize { get; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ConnectionCompleteUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        public ConnectionCompleteUnit(
            DataLink connection, 
            int miu = Constants.MaximumInformationUnit,
            int receiveWindowSize = Constants.DefaultReceiveWindowSize)
            : base(
                  connection, 
                  ProtocolDataUnitType.ConnectionComplete, 
                  null, 
                  ToBytes(miu, receiveWindowSize))
        {
            this.MaximumInformationUnit = miu;
            this.ReceiveWindowSize = receiveWindowSize;
        }

        /// <summary>
        /// Converts the parameters to bytes.
        /// </summary>
        /// <returns>The bytes, or null if the parameter list is null.</returns>
        private static byte[] ToBytes(int miu, int rw)
        {
            var parameters = new ParameterList();
            if (miu != Constants.MaximumInformationUnit)
            {
                parameters.Add(new MiuxParameter(miu));
            }

            if (rw != Constants.DefaultReceiveWindowSize)
            {
                parameters.Add(new ReceiveWindowSizeParameter(rw));
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

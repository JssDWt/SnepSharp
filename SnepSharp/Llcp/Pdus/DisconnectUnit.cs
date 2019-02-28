//
//  DisconnectUnit.cs
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
    /// <summary>
    /// Disconnect (DISC) unit. Is used to terminate a data link connection or 
    /// is used to deactivate the LLCP Link.
    /// </summary>
    internal class DisconnectUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DisconnectUnit"/> 
        /// class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        public DisconnectUnit(DataLink connection)
            : base(connection, ProtocolDataUnitType.Disconnect, null, null)
        {
        }
    }
}

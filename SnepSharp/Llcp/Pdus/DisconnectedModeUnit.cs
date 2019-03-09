//
//  DisconnectedModeUnit.cs
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
    /// Disconnected mode (DM) unit. Is used to report status indicating that 
    /// the LLC is logically disconnected from the data link connection 
    /// identified by the DSAP and SSAP address pair.
    /// </summary>
    internal class DisconnectedModeUnit : ProtocolDataUnit
    {
        public DisconnectReason Reason { get; }
        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.DisconnectedModeUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="reason">Disconnect reason.</param>
        public DisconnectedModeUnit(DataLink connection, DisconnectReason reason)
            : base(connection, ProtocolDataUnitType.DisconnectedMode, null, new byte[] { (byte)reason })
        {
            this.Reason = reason;
        }
    }
}

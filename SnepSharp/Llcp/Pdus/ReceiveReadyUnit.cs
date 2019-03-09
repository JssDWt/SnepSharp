//
//  ReceiveReadyUnit.cs
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
    /// Receive ready (RR) unit. is used by an LLC to acknowledge one or more 
    /// received I PDUs and indicate that the LLC is able to receive subsequent 
    /// I PDUs.
    /// </summary>
    internal class ReceiveReadyUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReceiveReadyUnit"/> 
        /// class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="sequence">Receive sequence number. Indicates all I pdus 
        /// up through that number have been processed and can be considered 
        /// acknowledged.</param>
        public ReceiveReadyUnit(DataLink connection, SequencePair sequence)
            : base(connection, ProtocolDataUnitType.ReceiveReady, sequence, null)
        {
        }
    }
}

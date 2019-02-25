//
//  InformationUnit.cs
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
    /// Information (I) unit. Is used to transfer service data units across a 
    /// data link connection.
    /// </summary>
    internal class InformationUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InformationUnit"/> 
        /// class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="sequence">Sequence number. Send part indicates the 
        /// sequence number for this pdu. Receive number indicates all PDUs up 
        /// through that number have been received by the service layer.</param>
        /// <param name="serviceData">Service data.</param>
        public InformationUnit(
            DataLink connection, 
            SequenceNumber sequence, 
            byte[] serviceData)
            : base(
                connection, 
                ProtocolDataUnitType.Information, 
                sequence, 
                serviceData)
        {
        }
    }
}

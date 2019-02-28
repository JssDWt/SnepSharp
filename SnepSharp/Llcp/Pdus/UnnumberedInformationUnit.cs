//
//  UnnumberedInformationUnit.cs
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

    /// <summary>
    /// Unnumbered information (UI) unit. Is used to transfer service data units
    /// to the peer LLC without prior establishment of a data link connection.
    /// </summary>
    internal class UnnumberedInformationUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="UnnumberedInformationUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="serviceData">Optional service data.</param>
        public UnnumberedInformationUnit(
            DataLink connection, 
            byte[] serviceData)
            : base(
                connection, 
                ProtocolDataUnitType.UnnumberedInformation, 
                null, 
                serviceData)
        {
        }
    }
}

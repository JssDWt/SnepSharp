//
//  SymmetryUnit.cs
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
    /// Symmetry unit (SYMM). Sent by an LLC whenever no other PDUs are 
    /// available for sending, to ensure symmetry.
    /// </summary>
    internal class SymmetryUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SymmetryUnit"/> class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        public SymmetryUnit(DataLink connection)
            : base(connection, ProtocolDataUnitType.Symmetry, null, null)
        {
        }
    }
}

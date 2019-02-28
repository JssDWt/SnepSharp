//
//  AggregatedFrameUnit.cs
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
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Aggregated frame (AGF) unit. MAY be used by the LLC Link Management 
    /// component to aggregate and transfer multiple LLC PDUs to the remote LLC 
    /// Link Management component in a single transmission.
    /// </summary>
    internal class AggregatedFrameUnit : ProtocolDataUnit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AggregatedFrameUnit"/> 
        /// class.
        /// </summary>
        /// <param name="connection">Data link connection.</param>
        /// <param name="aggregate">Aggregate of protocol data units.</param>
        public AggregatedFrameUnit(
            DataLink connection, 
            ICollection<ProtocolDataUnit> aggregate)
            : base(
                  connection, 
                  ProtocolDataUnitType.AggregatedFrame, 
                  null, 
                  ToBytes(aggregate))
        {

        }

        /// <summary>
        /// Converts the aggregate Protocol Data units to bytes.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="aggregate">The aggragate data units.</param>
        private static byte[] ToBytes(ICollection<ProtocolDataUnit> aggregate)
        {
            if (aggregate == null)
            {
                throw new ArgumentNullException(nameof(aggregate));
            }

            if (aggregate.Count < 2)
            {
                throw new ArgumentException(
                    "At least two subunits should be supplied.", 
                    nameof(aggregate));
            }

            if (aggregate.Any(
                pdu => pdu is AggregatedFrameUnit || pdu is SymmetryUnit))
            {
                throw new ArgumentException(
                    "Aggregate cannot contain aggregate or symmetry pdus.", 
                    nameof(aggregate));
            }

            // NOTE: first two bytes of each unit contain length, then data.
            throw new NotImplementedException();
        }

        /// <summary>
        /// Converts the specified bytes to Protocol Data units.
        /// </summary>
        /// <returns>The protocol data units.</returns>
        /// <param name="data">bytes containing protocol data units.</param>
        private static ICollection<ProtocolDataUnit> FromBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            throw new NotImplementedException();
        }
    }
}

//
//  PduCounter.cs
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

namespace SnepSharp.Llcp
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using SnepSharp.Llcp.Pdus;

    /// <summary>
    /// Used for counting sent and received <see cref="ProtocolDataUnit"/>
    /// of each <see cref="ProtocolDataUnitType"/>.
    /// </summary>
    public class PduCounter
    {
        /// <summary>
        /// Gets the sent <see cref="ProtocolDataUnit"/> count of each type.
        /// </summary>
        /// <value>The sent pdus.</value>
        public Dictionary<ProtocolDataUnitType, int> Sent { get; }
            = Empty();

        /// <summary>
        /// Gets the received <see cref="ProtocolDataUnit"/> count of each type.
        /// </summary>
        /// <value>The received.</value>
        public Dictionary<ProtocolDataUnitType, int> Received { get; }
            = Empty();

        /// <summary>
        /// Creates an empty initialized dictionary with a key for each
        /// <see cref="ProtocolDataUnitType"/>.
        /// </summary>
        /// <returns>The created dictionary.</returns>
        private static Dictionary<ProtocolDataUnitType, int> Empty()
        {
            var values = Enum.GetValues(typeof(ProtocolDataUnitType))
                .Cast<ProtocolDataUnitType>();

            var dict = new Dictionary<ProtocolDataUnitType, int>();
            foreach(var pduType in values)
            {
                dict.Add(pduType, 0);
            }

            return dict;
        }
    }
}

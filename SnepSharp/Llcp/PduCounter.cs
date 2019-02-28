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
using System;
using System.Collections.Generic;
using System.Linq;
using SnepSharp.Llcp.Pdus;

namespace SnepSharp.Llcp
{
    public class PduCounter
    {
        public Dictionary<ProtocolDataUnitType, int> Sent { get; }
            = Empty();

        public Dictionary<ProtocolDataUnitType, int> Received { get; }
            = Empty();

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

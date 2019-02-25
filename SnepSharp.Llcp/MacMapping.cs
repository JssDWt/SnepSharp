//
//  MacMapping.cs
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
    using SnepSharp.Llcp.Parameters;

    internal class MacMapping
    {
        public ParameterList Parameters { get; }

        /// <summary>
        /// Gets a value indicating whether the MAC allows parameter exchange.
        /// </summary>
        /// <value><c>true</c> if parameter exchange allowed; otherwise, <c>false</c>.</value>
        public bool ParameterExchangeAllowed { get; }

        /// <summary>
        /// Gets a value indicating whether the local LLCP is initiator.
        /// </summary>
        /// <value><c>true</c> if is initiator; <c>false</c> if is target.</value>
        public bool IsInitiator { get; }

        public MacMapping()
        {
        }
    }
}

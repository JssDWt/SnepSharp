//
//  ParameterList.cs
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

namespace SnepSharp.Llcp.Parameters
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a list of parameters.
    /// </summary>
    internal class ParameterList : List<Parameter>
    {
        /// <summary>
        /// Creates a byte representation of the parameter list.
        /// </summary>
        /// <returns>The bytes.</returns>
        public byte[] ToBytes()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Parses the bytes to create an instance of <see cref="ParameterList"/>.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="data">Parsed list.</param>
        public static ParameterList FromBytes(byte[] data)
        {
            throw new NotImplementedException();
        }
    }
}

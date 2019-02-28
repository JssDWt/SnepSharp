//
//  OptionParameter.cs
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

    /// <summary>
    /// The Option parameter communicates the link service class and the set of 
    /// options supported by the sending LLC.
    /// </summary>
    internal class OptionParameter : Parameter
    {
        /// <summary>
        /// Gets the supported link service class.
        /// </summary>
        /// <value>The link service class.</value>
        public LinkServiceClass Class { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptionParameter"/> 
        /// class.
        /// </summary>
        /// <param name="cls">The link service class.</param>
        public OptionParameter(LinkServiceClass cls)
            : base(ParameterType.Option, AsBytes(cls))
        {
            this.Class = cls;
        }

        /// <summary>
        /// Creates a byte representation of the <see cref="LinkServiceClass"/>.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="cls">The supported link service class.</param>
        private static byte[] AsBytes(LinkServiceClass cls)
        {
            byte result;
            switch (cls)
            {
                case LinkServiceClass.Unknown:            result = 0x00; break;
                case LinkServiceClass.Connectionless:     result = 0x01; break;
                case LinkServiceClass.ConnectionOriented: result = 0x02; break;
                case LinkServiceClass.Both:               result = 0x03; break;
                default:
                    throw new ArgumentException(
                        "Unknown LinkServiceClass.", 
                        nameof(cls));
            }

            return new byte[] { result };
        }
    }
}

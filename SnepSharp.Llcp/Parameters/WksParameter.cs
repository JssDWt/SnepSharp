//
//  WksParameter.cs
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
    /// The well-known service (WKS) parameter is a configuration parameter 
    /// that SHALL denote the binding of service listeners to well-known 
    /// service access point addresses and therefore the willingness of the 
    /// sender of the WKS parameter to accept PDUs on those SAPs.
    /// </summary>
    internal class WksParameter : Parameter
    {
        /// <summary>
        /// Gets the bound well-knownservice addresses.
        /// </summary>
        /// <value>The bound addresses.</value>
        public ICollection<LinkAddress> BoundAddresses { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WksParameter"/> class.
        /// </summary>
        /// <param name="boundAddresses">Bound addresses.</param>
        public WksParameter(ICollection<LinkAddress> boundAddresses)
            : base(ParameterType.WellKnownServiceList, AsBytes(boundAddresses))
        {
            this.BoundAddresses = boundAddresses;
        }

        /// <summary>
        /// Creates a byte representation of the addresses.
        /// Creating 16 bits, where the first bit represents address 15, and the 
        /// last bit address 0. '1' means the address is bound.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <exception cref="ArgumentException">Thrown when the address is not a 
        /// well-known address.</exception>
        private static byte[] AsBytes(ICollection<LinkAddress> boundAddresses)
        {
            var result = new byte[]
            {
                0x00,
                0x01 // Address 0 is always true.
            };

            foreach (byte address in boundAddresses)
            {
                switch (address)
                {
                    case 0x0F: result[0] |= 0x80; break;
                    case 0x0E: result[0] |= 0x40; break;
                    case 0x0D: result[0] |= 0x20; break;
                    case 0x0C: result[0] |= 0x10; break;
                    case 0x0B: result[0] |= 0x08; break;
                    case 0x0A: result[0] |= 0x04; break;
                    case 0x09: result[0] |= 0x02; break;
                    case 0x08: result[0] |= 0x01; break;
                    case 0x07: result[1] |= 0x80; break;
                    case 0x06: result[1] |= 0x40; break;
                    case 0x05: result[1] |= 0x20; break;
                    case 0x04: result[1] |= 0x10; break;
                    case 0x03: result[1] |= 0x08; break;
                    case 0x02: result[1] |= 0x04; break;
                    case 0x01: result[1] |= 0x02; break;
                    case 0x00: result[1] |= 0x01; break;
                    default:
                        throw new ArgumentException(
                            "Address value cannot be greater than 15 (4 bits).", 
                            nameof(boundAddresses));
                }
            }

            return result;
        }
    }
}

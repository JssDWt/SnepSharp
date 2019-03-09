//
//  LinkAddress.cs
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

    /// <summary>
    /// Link address.
    /// </summary>
    public struct LinkAddress : IEquatable<LinkAddress>, IEquatable<byte>
    {
        /// <summary>
        /// Address that designates the LLC link management component.
        /// </summary>
        public static readonly LinkAddress LlcLinkManagementComponentSap 
            = (LinkAddress)0x00;

        /// <summary>
        /// Designates the wellknown address for the Service Delivery Protocol.
        /// </summary>
        public static readonly LinkAddress ServiceDiscoveryProtocolSap 
            = (LinkAddress)0x01;

        /// <summary>
        /// Addresses for well-known service access points defined in the NFC 
        /// Forum Assigned Values Registry [NFCREG].
        /// </summary>
        public static readonly IReadOnlyCollection<LinkAddress> WellknownSaps 
            = new HashSet<LinkAddress>(
                Enumerable.Range(0x00, 0x0F).Cast<LinkAddress>());

        /// <summary>
        /// Addresses to be assigned by the local LLC to services registered by 
        /// the local service environment. These registrations SHALL be made 
        /// available by the local Service Discovery Protocol (SDP) instance for 
        /// discovery and use by a remote LLC.
        /// </summary>
        public static readonly IReadOnlyCollection<LinkAddress> DiscoverableSaps
            = new HashSet<LinkAddress>(
                Enumerable.Range(0x10, 0x1F).Cast<LinkAddress>());

        /// <summary>
        /// Addresses to be assigned by the local LLC as the result of an upper 
        /// layer service request. SHALL NOT be available for discovery using 
        /// the Service Discovery Protocol (SDP).
        /// </summary>
        public static readonly IReadOnlyCollection<LinkAddress> UpperLayerSaps 
            = new HashSet<LinkAddress>(
                Enumerable.Range(0x20, 0x3F).Cast<LinkAddress>());

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>The address.</value>
        public byte Address { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LinkAddress"/> struct.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the 
        /// address does not fit 6 bits (0 to 63)</exception>
        public LinkAddress(byte address)
        {
            if (address < 0 || address > 63)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(address), 
                    "Address must fit in 6 bits (0 <= address <= 63)");
            }

            this.Address = address;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to 
        /// the current <see cref="LinkAddress"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the 
        /// current <see cref="LinkAddress"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal 
        /// to the current <see cref="LinkAddress"/>;
        /// otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is LinkAddress)
            {
                return this.Equals((LinkAddress)obj);
            }

            if (obj is byte)
            {
                return this.Equals((byte)obj);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="Llcp.LinkAddress"/> is 
        /// equal to the current <see cref="T:Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="other">The <see cref="Llcp.LinkAddress"/> to compare 
        /// with the current <see cref="T:Llcp.LinkAddress"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Llcp.LinkAddress"/> 
        /// is equal to the current
        /// <see cref="T:Llcp.LinkAddress"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(LinkAddress other) 
            => this.Address.Equals(other.Address);

        /// <summary>
        /// Determines whether the specified <see cref="byte"/> is equal to the 
        /// current <see cref="T:Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="other">The <see cref="byte"/> to compare with the 
        /// current <see cref="T:Llcp.LinkAddress"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="byte"/> is equal to 
        /// the current <see cref="T:Llcp.LinkAddress"/>;
        /// otherwise, <c>false</c>.</returns>
        public bool Equals(byte other) => this.Address.Equals(other);

        /// <summary>
        /// Serves as a hash function for a <see cref="T:Llcp.LinkAddress"/> 
        /// object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in 
        /// hashing algorithms and data structures such as a hash table.
        /// </returns>
        public override int GetHashCode() => this.Address.GetHashCode();

        /// <summary>
        /// Determines whether a specified instance of 
        /// <see cref="Llcp.LinkAddress"/> is not equal to another specified 
        /// <see cref="Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LinkAddress"/> to 
        /// compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LinkAddress"/> to 
        /// compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(LinkAddress l1, LinkAddress l2) 
            => !l1.Equals(l2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="LinkAddress"/> 
        /// is not equal to another specified <see cref="byte"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LinkAddress"/> to 
        /// compare.</param>
        /// <param name="l2">The second <see cref="byte"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(LinkAddress l1, byte l2) 
            => !l1.Equals(l2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="byte"/> is not 
        /// equal to another specified <see cref="Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="byte"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LinkAddress"/> to 
        /// compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(byte l1, LinkAddress l2) 
            => !l2.Equals(l1);

        /// <summary>
        /// Determines whether a specified instance of <see cref="LinkAddress"/> 
        /// is equal to another specified <see cref="Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LinkAddress"/> to 
        /// compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LinkAddress"/> to 
        /// compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(LinkAddress l1, LinkAddress l2) 
            => l1.Equals(l2);
        
        /// <summary>
        /// Determines whether a specified instance of <see cref="LinkAddress"/> 
        /// is equal to another specified <see cref="byte"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LinkAddress"/> to 
        /// compare.</param>
        /// <param name="l2">The second <see cref="byte"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(LinkAddress l1, byte l2) 
            => l1.Equals(l2);
        
        /// <summary>
        /// Determines whether a specified instance of <see cref="byte"/> is 
        /// equal to another specified <see cref="Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="byte"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LinkAddress"/> to 
        /// compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(byte l1, LinkAddress l2) 
            => l2.Equals(l1);

        /// <summary>
        /// Implicit <see cref="LinkAddress"/> to <see cref="byte"/> conversion 
        /// operator.
        /// </summary>
        /// <returns>A link address byte.</returns>
        /// <param name="address">The <see cref="LinkAddress"/> to convert to 
        /// <see cref="byte"/></param>
        public static implicit operator byte(LinkAddress address) 
            => address.Address;

        /// <summary>
        /// Explicit <see cref="byte"/> to <see cref="LinkAddress"/> conversion 
        /// operator.
        /// </summary>
        /// <returns>A link address </returns>
        /// <param name="b">The <see cref="byte"/> to convert to 
        /// <see cref="LinkAddress"/></param>
        public static explicit operator LinkAddress(byte b) 
            => new LinkAddress(b);
    }
}

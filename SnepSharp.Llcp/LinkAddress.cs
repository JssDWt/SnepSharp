namespace SnepSharp.Llcp
{
    using System;
    using System.Collections;
    using System.Linq;

    /// <summary>
    /// Link address.
    /// </summary>
    public struct LinkAddress : IEquatable<LinkAddress>, IEquatable<byte>
    {
        /// <summary>
        /// Address that designates the LLC link management component.
        /// </summary>
        public const byte LlcLinkManagementComponentSap = 0x00;

        /// <summary>
        /// Designates the wellknown address for the Service Delivery Protocol.
        /// </summary>
        public const byte ServiceDiscoveryProtocolSap = 0x01;

        /// <summary>
        /// Addresses for well-known service access points defined in the NFC Forum Assigned Values Registry [NFCREG].
        /// </summary>
        public static readonly byte[] WellknownSaps = Enumerable.Range(0x02, 0x0F).Cast<byte>().ToArray();

        /// <summary>
        /// Addresses to be assigned by the local LLC to services registered by the local service environment.
        /// These registrations SHALL be made available by the local Service Discovery Protocol (SDP) instance for discovery and use by a remote LLC.
        /// </summary>
        public static readonly byte[] DiscoverableSaps = Enumerable.Range(0x10, 0x1F).Cast<byte>().ToArray();

        /// <summary>
        /// Addresses to be assigned by the local LLC as the result of an upper layer service request.
        /// SHALL NOT be available for discovery using the Service Discovery Protocol (SDP).
        /// </summary>
        public static readonly byte[] UpperLayerSaps = Enumerable.Range(0x20, 0x3F).Cast<byte>().ToArray();

        /// <summary>
        /// Gets the address.
        /// </summary>
        /// <value>The address.</value>
        public byte Address { get; }

        /// <summary>
        /// Gets the address bits.
        /// </summary>
        /// <value>The address bits.</value>
        public BitArray BitAddress { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.LinkAddress"/> struct.
        /// </summary>
        /// <param name="address">The address.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the address does not fit 6 bits (0 to 63)</exception>
        public LinkAddress(byte address)
        {
            if (address < 0 || address > 63)
            {
                throw new ArgumentOutOfRangeException(nameof(address), "Address must fit in 6 bits (0 <= address <= 63)");
            }

            this.Address = address;
            this.BitAddress = new BitArray(new byte[] { address })
            {
                Length = 6
            };
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:Llcp.LinkAddress"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="T:Llcp.LinkAddress"/>;
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
        /// Determines whether the specified <see cref="Llcp.LinkAddress"/> is equal to the current <see cref="T:Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="other">The <see cref="Llcp.LinkAddress"/> to compare with the current <see cref="T:Llcp.LinkAddress"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Llcp.LinkAddress"/> is equal to the current
        /// <see cref="T:Llcp.LinkAddress"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(LinkAddress other) => this.Address.Equals(other.Address);

        /// <summary>
        /// Determines whether the specified <see cref="byte"/> is equal to the current <see cref="T:Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="other">The <see cref="byte"/> to compare with the current <see cref="T:Llcp.LinkAddress"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="byte"/> is equal to the current <see cref="T:Llcp.LinkAddress"/>;
        /// otherwise, <c>false</c>.</returns>
        public bool Equals(byte other) => this.Address.Equals(other);

        /// <summary>
        /// Serves as a hash function for a <see cref="T:Llcp.LinkAddress"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => this.Address.GetHashCode();

        /// <summary>
        /// Determines whether a specified instance of <see cref="Llcp.LinkAddress"/> is not equal to another specified <see cref="Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LinkAddress"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LinkAddress"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(LinkAddress l1, LinkAddress l2) => !l1.Equals(l2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="Llcp.LinkAddress"/> is not equal to another specified <see cref="byte"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LinkAddress"/> to compare.</param>
        /// <param name="l2">The second <see cref="byte"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(LinkAddress l1, byte l2) => !l1.Equals(l2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="byte"/> is not equal to another specified <see cref="Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="byte"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LinkAddress"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(byte l1, LinkAddress l2) => !l2.Equals(l1);

        /// <summary>
        /// Determines whether a specified instance of <see cref="Llcp.LinkAddress"/> is equal to another specified <see cref="Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LinkAddress"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LinkAddress"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(LinkAddress l1, LinkAddress l2) => l1.Equals(l2);
        
        /// <summary>
        /// Determines whether a specified instance of <see cref="Llcp.LinkAddress"/> is equal to another specified <see cref="byte"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LinkAddress"/> to compare.</param>
        /// <param name="l2">The second <see cref="byte"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(LinkAddress l1, byte l2) => l1.Equals(l2);
        
        /// <summary>
        /// Determines whether a specified instance of <see cref="byte"/> is equal to another specified <see cref="Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="byte"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LinkAddress"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(byte l1, LinkAddress l2) => l2.Equals(l1);

        /// <summary>
        /// Implicit <see cref="LinkAddress"/> to <see cref="byte"/> conversion operator.
        /// </summary>
        /// <returns>A link address byte.</returns>
        /// <param name="address">The <see cref="LinkAddress"/> to convert to <see cref="byte"/></param>
        public static implicit operator byte(LinkAddress address) => address.Address;

        /// <summary>
        /// Explicit <see cref="byte"/> to <see cref="LinkAddress"/> conversion operator.
        /// </summary>
        /// <returns>A link address </returns>
        /// <param name="b">The <see cref="byte"/> to convert to <see cref="LinkAddress"/></param>
        public static explicit operator LinkAddress(byte b) => new LinkAddress(b);
    }
}

namespace SnepSharp.Llcp
{
    internal struct LlcpVersion
    {
        public static readonly LlcpVersion V10 = (LlcpVersion)0x10;

        public int Major { get; }
        public int Minor { get; }
        public byte Version { get; }

        public LlcpVersion(byte version)
        {
            this.Version = version;
            this.Major = (version & 0xF0) >> 4;
            this.Minor = (version & 0x0F);
        }

        public LlcpVersion(int major, int minor)
        {
            this.Major = major;
            this.Minor = minor;
            this.Version = (byte)((major << 4) | minor);
        }

        /// <summary>
        /// Determines whether the specified <see cref="Llcp.LlcpVersion"/> is equal to the current <see cref="T:Llcp.LlcpVersion"/>.
        /// </summary>
        /// <param name="other">The <see cref="Llcp.LlcpVersion"/> to compare with the current <see cref="T:Llcp.LlcpVersion"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Llcp.LlcpVersion"/> is equal to the current
        /// <see cref="T:Llcp.LlcpVersion"/>; otherwise, <c>false</c>.</returns>
        public bool Equals(LlcpVersion other) => this.Version.Equals(other.Version);

        /// <summary>
        /// Determines whether the specified <see cref="byte"/> is equal to the current <see cref="T:Llcp.LlcpVersion"/>.
        /// </summary>
        /// <param name="other">The <see cref="byte"/> to compare with the current <see cref="T:Llcp.LlcpVersion"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="byte"/> is equal to the current <see cref="T:Llcp.LlcpVersion"/>;
        /// otherwise, <c>false</c>.</returns>
        public bool Equals(byte other) => this.Version.Equals(other);

        /// <summary>
        /// Serves as a hash function for a <see cref="T:Llcp.LlcpVersion"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a
        /// hash table.</returns>
        public override int GetHashCode() => this.Version.GetHashCode();

        /// <summary>
        /// Determines whether a specified instance of <see cref="Llcp.LlcpVersion"/> is not equal to another specified <see cref="Llcp.LlcpVersion"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LlcpVersion"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LlcpVersion"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(LlcpVersion l1, LlcpVersion l2) => !l1.Equals(l2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="Llcp.LlcpVersion"/> is not equal to another specified <see cref="byte"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LlcpVersion"/> to compare.</param>
        /// <param name="l2">The second <see cref="byte"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(LlcpVersion l1, byte l2) => !l1.Equals(l2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="byte"/> is not equal to another specified <see cref="Llcp.LlcpVersion"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="byte"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LlcpVersion"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(byte l1, LlcpVersion l2) => !l2.Equals(l1);

        /// <summary>
        /// Determines whether a specified instance of <see cref="Llcp.LinkAddress"/> is equal to another specified <see cref="Llcp.LlcpVersion"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LlcpVersion"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LlcpVersion"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(LlcpVersion l1, LlcpVersion l2) => l1.Equals(l2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="Llcp.LlcpVersion"/> is equal to another specified <see cref="byte"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.LlcpVersion"/> to compare.</param>
        /// <param name="l2">The second <see cref="byte"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(LlcpVersion l1, byte l2) => l1.Equals(l2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="byte"/> is equal to another specified <see cref="Llcp.LlcpVersion"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="byte"/> to compare.</param>
        /// <param name="l2">The second <see cref="Llcp.LlcpVersion"/> to compare.</param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(byte l1, LlcpVersion l2) => l2.Equals(l1);

        /// <summary>
        /// Implicit <see cref="LlcpVersion"/> to <see cref="byte"/> conversion operator.
        /// </summary>
        /// <returns>A llcp version byte.</returns>
        /// <param name="version">The <see cref="LlcpVersion"/> to convert to <see cref="byte"/></param>
        public static implicit operator byte(LlcpVersion version) => version.Version;

        /// <summary>
        /// Explicit <see cref="byte"/> to <see cref="LlcpVersion"/> conversion operator.
        /// </summary>
        /// <returns>An llcp version.</returns>
        /// <param name="b">The <see cref="byte"/> to convert to <see cref="LlcpVersion"/></param>
        public static explicit operator LlcpVersion(byte b) => new LlcpVersion(b);
    }
}

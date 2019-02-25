//
//  LlcpVersion.cs
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

    /// <summary>
    /// Llcp version.
    /// </summary>
    internal struct LlcpVersion : IEquatable<LlcpVersion>, IEquatable<byte>
    {
        /// <summary>
        /// The deafult LLCP version; 1.0.
        /// </summary>
        public static readonly LlcpVersion V10 = (LlcpVersion)0x10;

        /// <summary>
        /// Gets the major version.
        /// </summary>
        /// <value>The major version.</value>
        public int Major { get; }

        /// <summary>
        /// Gets the minor version.
        /// </summary>
        /// <value>The minor version.</value>
        public int Minor { get; }

        /// <summary>
        /// Gets the version, as transmitted over LLCP.
        /// </summary>
        /// <value>The version.</value>
        public byte Version { get; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:SnepSharp.Llcp.LlcpVersion"/> struct.
        /// </summary>
        /// <param name="version">Version.</param>
        public LlcpVersion(byte version)
        {
            this.Version = version;
            this.Major = (version & 0xF0) >> 4;
            this.Minor = (version & 0x0F);
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="T:SnepSharp.Llcp.LlcpVersion"/> struct, with the 
        /// specified major and minor version.
        /// </summary>
        /// <param name="major">Major version.</param>
        /// <param name="minor">Minor version.</param>
        public LlcpVersion(int major, int minor)
        {
            if (major > 0x0F || major < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(major),
                    "Version number should be between 0 and 15, inclusive.");
            }

            if (minor > 0x0F || minor < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(minor),
                    "Version number should be between 0 and 15, inclusive.");
            }

            this.Major = major;
            this.Minor = minor;
            this.Version = (byte)((major << 4) | minor);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to the current <see cref="T:Llcp.LinkAddress"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the current <see cref="T:Llcp.LinkAddress"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal to the current <see cref="T:Llcp.LinkAddress"/>;
        /// otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is LlcpVersion)
            {
                return this.Equals((LlcpVersion)obj);
            }

            if (obj is byte)
            {
                return this.Equals((byte)obj);
            }

            return false;
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

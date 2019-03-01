//
//  DataLink.cs
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
    /// Represents a data link in the data link PDU. Can be a logical data link 
    /// in connectionless transport, or a data link connection in 
    /// connection-oriented transport.
    /// </summary>
    public struct DataLink : IEquatable<DataLink>
    {
        /// <summary>
        /// An empty datalink, with dsap and ssap both 0.
        /// </summary>
        public static DataLink Empty 
            = new DataLink((LinkAddress)0, (LinkAddress)0);

        /// <summary>
        /// Gets the Destination Service Access Point (DSAP).
        /// </summary>
        /// <value>The Destination Service Access Point.</value>
        public LinkAddress Destination { get; }

        /// <summary>
        /// Gets the Source Service Access Point (SSAP).
        /// </summary>
        /// <value>The Source Service Access Point.</value>
        public LinkAddress Source { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DataLink"/> struct.
        /// </summary>
        /// <param name="source">The Source Service Access Point.</param>
        /// <param name="destination">The Destination Service Access Point.</param>
        public DataLink(LinkAddress source, LinkAddress destination)
        {
            this.Source = source;
            this.Destination = destination;
        }

        /// <summary>
        /// Determines whether the specified <see cref="object"/> is equal to 
        /// the current <see cref="DataLink"/>.
        /// </summary>
        /// <param name="obj">The <see cref="object"/> to compare with the 
        /// current <see cref="DataLink"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="object"/> is equal 
        /// to the current <see cref="DataLink"/>; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is DataLink)) return false;
            return this.Equals((DataLink)obj);
        }

        /// <summary>
        /// Determines whether the specified <see cref="DataLink"/> is equal to 
        /// the current <see cref="DataLink"/>.
        /// </summary>
        /// <param name="other">The <see cref="DataLink"/> to compare with the 
        /// current <see cref="DataLink"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="DataLink"/> is 
        /// equal to the current <see cref="DataLink"/>; otherwise, 
        /// <c>false</c>.</returns>
        public bool Equals(DataLink other)
            => this.Destination == other.Destination && this.Source == other.Source;

        /// <summary>
        /// Serves as a hash function for a <see cref="DataLink"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in 
        /// hashing algorithms and data structures such as a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            int hash = 4663;
            unchecked
            {
                hash = hash * 6053 + this.Destination.GetHashCode();
                hash = hash * 6053 + this.Source.GetHashCode();
            }

            return hash;
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="DataLink"/> is 
        /// equal to another specified <see cref="Llcp.DataLink"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.DataLink"/> to compare.
        /// </param>
        /// <param name="l2">The second <see cref="Llcp.DataLink"/> to compare.
        /// </param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(DataLink l1, DataLink l2)
        {
            return l1.Equals(l2);
        }

        /// <summary>
        /// Determines whether a specified instance of <see cref="DataLink"/> is 
        /// not equal to another specified <see cref="Llcp.DataLink"/>.
        /// </summary>
        /// <param name="l1">The first <see cref="Llcp.DataLink"/> to compare.
        /// </param>
        /// <param name="l2">The second <see cref="Llcp.DataLink"/> to compare.
        /// </param>
        /// <returns><c>true</c> if <c>l1</c> and <c>l2</c> are not equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(DataLink l1, DataLink l2)
        {
            return !l1.Equals(l2);
        }
    }
}

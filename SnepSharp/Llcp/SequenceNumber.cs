//
//  SequenceNumber.cs
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
    /// Sequence number representing a 4 bit number, that rolls over after 15, 
    /// to 0.
    /// </summary>
    public struct SequenceNumber : IEquatable<SequenceNumber>, IEquatable<int>
    {
        /// <summary>
        /// The sequence, can be from 0 to 15.
        /// </summary>
        private readonly int sequence;

        /// <summary>
        /// Initializes a new instance of the <see cref="SequenceNumber"/> 
        /// struct.
        /// </summary>
        /// <param name="sequence">Sequence number.</param>
        /// <remarks>If the sequence number is larger than 15, the modulus 16 is
        /// taken to represent the sequence number.</remarks>
        public SequenceNumber(int sequence)
        {
            if (sequence < 0) throw new ArgumentOutOfRangeException(
                nameof(sequence), 
                "Cannot be negative.");

            this.sequence = sequence % 16;
        }

        /// <summary>
        /// Serves as a hash function for a <see cref="SequenceNumber"/> object.
        /// </summary>
        /// <returns>A hash code for this instance that is suitable for use in 
        /// hashing algorithms and data structures such as a hash table.
        /// </returns>
        public override int GetHashCode() => this.sequence.GetHashCode();

        /// <summary>
        /// Returns a <see cref="String"/> that represents the current 
        /// <see cref="SequenceNumber"/>.
        /// </summary>
        /// <returns>A <see cref="String"/> that represents the current 
        /// <see cref="SequenceNumber"/>.</returns>
        public override string ToString() => this.sequence.ToString();

        /// <summary>
        /// Determines whether the specified <see cref="Object"/> is equal to 
        /// the current <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="obj">The <see cref="Object"/> to compare with the 
        /// current <see cref="SequenceNumber"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Object"/> is equal 
        /// to the current <see cref="SequenceNumber"/>; otherwise, 
        /// <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is int || obj is byte)
            {
                return this.Equals((int)obj);
            }

            if (obj is SequenceNumber)
            {
                return this.Equals((SequenceNumber)obj);
            }

            return false;
        }

        /// <summary>
        /// Determines whether the specified <see cref="SequenceNumber"/> is 
        /// equal to the current <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="other">The <see cref="SequenceNumber"/> to compare with 
        /// the current <see cref="SequenceNumber"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="SequenceNumber"/> 
        /// is equal to the current <see cref="SequenceNumber"/>; otherwise, 
        /// <c>false</c>.</returns>
        public bool Equals(SequenceNumber other)
            => this.sequence == other.sequence;

        /// <summary>
        /// Determines whether the specified <see cref="Int32"/> is equal to the 
        /// current <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="other">The <see cref="Int32"/> to compare with the 
        /// current <see cref="SequenceNumber"/>.</param>
        /// <returns><c>true</c> if the specified <see cref="Int32"/> is equal 
        /// to the current <see cref="SequenceNumber"/>; otherwise, 
        /// <c>false</c>.</returns>
        public bool Equals(int other) => this.sequence == (other % 16);

        /// <summary>
        /// Implicit <see cref="SequenceNumber"/> to <see cref="int"/> 
        /// conversion operator.
        /// </summary>
        /// <returns>A sequence number integer.</returns>
        /// <param name="sequence">The <see cref="SequenceNumber"/> to convert 
        /// to <see cref="int"/></param>
        public static implicit operator int(SequenceNumber sequence)
            => sequence.sequence;

        /// <summary>
        /// Explicit <see cref="int"/> to <see cref="SequenceNumber"/> 
        /// conversion operator.
        /// </summary>
        /// <returns>A sequence number.</returns>
        /// <param name="i">The <see cref="int"/> to convert to 
        /// <see cref="SequenceNumber"/></param>
        public static explicit operator SequenceNumber(int i)
            => new SequenceNumber(i);

        /// <summary>
        /// Determines whether a specified instance of 
        /// <see cref="SequenceNumber"/> is not equal to another specified 
        /// <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="SequenceNumber"/> to 
        /// compare.</param>
        /// <param name="s2">The second <see cref="SequenceNumber"/> to 
        /// compare.</param>
        /// <returns><c>true</c> if <c>s1</c> and <c>s2</c> are not equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(SequenceNumber s1, SequenceNumber s2)
            => !s1.Equals(s2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="SequenceNumber"/> 
        /// is not equal to another specified <see cref="int"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="SequenceNumber"/> to 
        /// compare.</param>
        /// <param name="s2">The second <see cref="int"/> to compare.</param>
        /// <returns><c>true</c> if <c>s1</c> and <c>s2</c> are not equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(SequenceNumber s1, int s2)
            => !s1.Equals(s2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="int"/> is not 
        /// equal to another specified <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="int"/> to compare.</param>
        /// <param name="s2">The second <see cref="SequenceNumber"/> to 
        /// compare.</param>
        /// <returns><c>true</c> if <c>s1</c> and <c>s2</c> are not equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator !=(int s1, SequenceNumber s2)
            => !s2.Equals(s1);

        /// <summary>
        /// Determines whether a specified instance of <see cref="SequenceNumber"/> 
        /// is equal to another specified <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="SequenceNumber"/> to 
        /// compare.</param>
        /// <param name="s2">The second <see cref="SequenceNumber"/> to 
        /// compare.</param>
        /// <returns><c>true</c> if <c>s1</c> and <c>s2</c> are equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(SequenceNumber s1, SequenceNumber s2)
            => s1.Equals(s2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="SequenceNumber"/> 
        /// is equal to another specified <see cref="int"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="SequenceNumber"/> to 
        /// compare.</param>
        /// <param name="s2">The second <see cref="int"/> to compare.</param>
        /// <returns><c>true</c> if <c>s1</c> and <c>s2</c> are equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(SequenceNumber s1, int s2)
            => s1.Equals(s2);

        /// <summary>
        /// Determines whether a specified instance of <see cref="int"/> is 
        /// equal to another specified <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="int"/> to compare.</param>
        /// <param name="s2">The second <see cref="SequenceNumber"/> to 
        /// compare.</param>
        /// <returns><c>true</c> if <c>s1</c> and <c>s2</c> are equal; 
        /// otherwise, <c>false</c>.</returns>
        public static bool operator ==(int s1, SequenceNumber s2)
            => s2.Equals(s1);

        /// <summary>
        /// Adds a <see cref="SequenceNumber"/> to a 
        /// <see cref="SequenceNumber"/>, yielding a new 
        /// <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="s1">The first <see cref="SequenceNumber"/> to add.
        /// </param>
        /// <param name="s2">The second <see cref="SequenceNumber"/> to add.
        /// </param>
        /// <returns>The <see cref="SequenceNumber"/> that is the sum of the 
        /// values of <c>s1</c> and <c>s2</c>.</returns>
        public static SequenceNumber operator +(
            SequenceNumber s1, 
            SequenceNumber s2)
            => new SequenceNumber(s1.sequence + s2.sequence);

        /// <summary>
        /// Subtracts a <see cref="SequenceNumber"/> from a 
        /// <see cref="SequenceNumber"/>, yielding a new 
        /// <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="s1">The <see cref="SequenceNumber"/> to subtract from 
        /// (the minuend).</param>
        /// <param name="s2">The <see cref="SequenceNumber"/> to subtract 
        /// (the subtrahend).</param>
        /// <returns>The <see cref="SequenceNumber"/> that is the <c>s1</c> 
        /// minus <c>s2</c>.</returns>
        public static SequenceNumber operator -(
            SequenceNumber s1,
            SequenceNumber s2)
            => Substract(s1, s2.sequence);

        /// <summary>
        /// Increments the <see cref="SequenceNumber"/> by 1, yielding
        /// a new <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="s">The <see cref="SequenceNumber"/> to increment.</param>
        public static SequenceNumber operator ++(SequenceNumber s)
            => new SequenceNumber(s.sequence + 1);

        /// <summary>
        /// Decrements the <see cref="SequenceNumber"/> by 1, yielding
        /// a new <see cref="SequenceNumber"/>.
        /// </summary>
        /// <param name="s">The <see cref="SequenceNumber"/> to decrement.
        /// </param>
        public static SequenceNumber operator --(SequenceNumber s)
            => Substract(s, 1);

        /// <summary>
        /// Substracts the <see cref="Int32"/> from the 
        /// <see cref="SequenceNumber"/>, yielding a new 
        /// <see cref="SequenceNumber"/>.
        /// </summary>
        /// <returns>The <see cref="SequenceNumber"/> that is the <c>s</c> 
        /// minus <c>i</c>.</returns>
        /// <param name="s">The sequence number.</param>
        /// <param name="i">The amount to substract. Cannot be more than 15.
        /// </param>
        private static SequenceNumber Substract(SequenceNumber s, int i)
        {
            int result = s.sequence - i;
            if (result < 0) result += 16;
            return new SequenceNumber(s);
        }
    }
}

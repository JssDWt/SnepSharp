//
//  IntExtensions.cs
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

namespace SnepSharp.Common
{
    using System;

    /// <summary>
    /// Class providing useful extension methods for <see cref="int"/>.
    /// </summary>
    public static class IntExtensions
    {
        /// <summary>
        /// Creates a byte[] representation of the specified <see cref="int"/>,
        /// with the most significant digits first.
        /// </summary>
        /// <returns>The byte representation of the <see cref="int"/>.</returns>
        /// <param name="intValue"><see cref="int"/> value.</param>
        public static byte[] ToByteArray(this int intValue)
        {
            byte[] intBytes = BitConverter.GetBytes(intValue);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(intBytes);
            return intBytes;
        }

        public static int FromByteArray(byte[] bytes, int startIndex = 0)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (BitConverter.IsLittleEndian)
            {
                // Reverse the bytes in a copy
                var copy = new byte[]
                {
                    bytes[startIndex + 1],
                    bytes[startIndex]
                };

                return BitConverter.ToInt16(copy, 0);
            }

            return BitConverter.ToInt16(bytes, startIndex);
        }
    }
}

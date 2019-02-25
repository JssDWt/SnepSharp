//
//  MiuxParameter.cs
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
    using SnepSharp.Common;

    /// <summary>
    /// Maximum Information Unit Extension (MIUX) parameter. Is the maximum 
    /// number of octets in the information field of an LLC PDU that the local 
    /// LLC is able to receive. The default MIU is 128.
    /// </summary>
    internal class MiuxParameter : Parameter
    {
        /// <summary>
        /// The maximum size for the Maximum Information Unit Extension (MIUX).
        /// Should fit in 11 bits.
        /// </summary>
        const int MaximumSize = 2047;

        /// <summary>
        /// Gets the size extension, or the number of octets by which an 
        /// information field MAY exceed the default maximum size.
        /// </summary>
        /// <value>The size extension.</value>
        public int SizeExtension { get; }

        /// <summary>
        /// Gets the actual Maximum Information Unit, as negotiated by this 
        /// parameter.
        /// </summary>
        /// <value>The actual Maximum Information Unit.</value>
        public int ActualMiu 
            => this.SizeExtension + Constants.MaximumInformationUnit;

        /// <summary>
        /// Initializes a new instance of the <see cref="MiuxParameter"/> class.
        /// </summary>
        /// <param name="sizeExtension">size extension, or the number of octets 
        /// by which an information field MAY exceed the default maximum size.
        /// </param>
        public MiuxParameter(int sizeExtension)
            : base(ParameterType.MiuxExtension, AsBytes(sizeExtension))
        {
            this.SizeExtension = sizeExtension;
        }

        /// <summary>
        /// Creates a byte representation of the size extension.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="sizeExtension">Size extension.</param>
        private static byte[] AsBytes(int sizeExtension)
        {
            if (sizeExtension < 0)
            {
                throw new ArgumentException(
                    "Must be greater than zero.",
                    nameof(sizeExtension));
            }

            if (sizeExtension > MaximumSize)
            {
                throw new ArgumentException(
                    $"Size extension cannot be greater than {MaximumSize}", 
                    nameof(sizeExtension));
            }

            var bytes = sizeExtension.ToByteArray();
            int length = bytes.Length;
            var result = new byte[2];
            result[0] = bytes[length - 2];
            result[1] = bytes[length - 1];
            return result;
        }
    }
}

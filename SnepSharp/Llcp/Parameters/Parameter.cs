//
//  Parameter.cs
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
    /// <summary>
    /// Represents an LLCP parameter.
    /// </summary>
    public abstract class Parameter
    {
        /// <summary>
        /// Gets the parameter type.
        /// </summary>
        /// <value>The type of the parameter.</value>
        public ParameterType Type { get; }

        /// <summary>
        /// Gets the length of the parameter bytes.
        /// </summary>
        /// <value>The length of the parameter bytes.</value>
        public byte Length { get; }

        /// <summary>
        /// The parameter value as byte array.
        /// </summary>
        private readonly byte[] value;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parameter"/> class.
        /// </summary>
        /// <param name="type">Parameter type.</param>
        /// <param name="value">Parameter value.</param>
        protected Parameter(ParameterType type, byte[] value)
        {
            this.Type = type;
            this.value = value;
            this.Length = (byte)(this.value?.Length ?? 0);
        }

        /// <summary>
        /// Creates a byte representation of the parameter, to send over LLCP.
        /// </summary>
        /// <returns>The byte array.</returns>
        public byte[] ToByteArray()
        {
            var result = new byte[this.Length + 2];
            result[0] = (byte)this.Type;
            result[1] = this.Length;
            if (this.value != null)
            {
                this.value.CopyTo(result, 2);
            }

            return result;
        }
    }
}

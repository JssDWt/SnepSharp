//
//  INdefMessage.cs
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

namespace SnepSharp.Ndef
{
    using System.IO;

    /// <summary>
    /// Represents an NDEF message.
    /// </summary>
    public interface INdefMessage
    {
        /// <summary>
        /// Gets the length of the byte encoded message in bytes.
        /// </summary>
        /// <value>The length of the message in bytes.</value>
        int Length { get; }

        /// <summary>
        /// Creates a <see cref="Stream"/> representation of the message.
        /// </summary>
        /// <returns>The message stream.</returns>
        Stream AsStream();
    }
}

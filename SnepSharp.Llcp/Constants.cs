//
//  Constants.cs
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
    using System.Text;

    /// <summary>
    /// Constants for the Logical Link Control Protocol.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The maximum information units (octets) to send in a PDU.
        /// </summary>
        public const int MaximumInformationUnit = 128;

        /// <summary>
        /// The default timeout in milliseconds.
        /// </summary>
        public const int DefaultTimeout = 100;

        /// <summary>
        /// The default receive window size.
        /// </summary>
        public const int DefaultReceiveWindowSize = 1;

        /// <summary>
        /// The default encoding to encode strings with.
        /// </summary>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;
    }
}

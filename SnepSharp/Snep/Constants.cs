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

namespace SnepSharp.Snep
{
    /// <summary>
    /// Class containing constants to use thoughout the library.
    /// </summary>
    internal static class Constants
    {
        /// <summary>
        /// The default snep version.
        /// </summary>
        public const SnepVersion DefaultSnepVersion = SnepVersion.V10;

        /// <summary>
        /// The length of the snep header.
        /// </summary>
        public const int SnepHeaderLength = 6;

        /// <summary>
        /// The default maximum receive buffer size.
        /// Defaults to 16 KiB.
        /// </summary>
        public const int DefaultMaxReceiveBufferSize = 1024 * 16;

        /// <summary>
        /// The default maximum response size. Defaults to 1 GiB.
        /// </summary>
        public const int DefaultMaxResponseSize = 1024 * 1024 * 1024;
    }
}

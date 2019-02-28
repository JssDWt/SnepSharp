//
//  SnepGetRequest.cs
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

namespace SnepSharp.Snep.Messages
{
    using System;
    using System.IO;
    using SnepSharp.Common;
    using SnepSharp.Ndef;

    /// <summary>
    /// Snep get request message. To request a resource from the snep server.
    /// </summary>
    internal class SnepGetRequest : SnepRequest
    {
        /// <summary>
        /// Gets the maximum accepted response length.
        /// </summary>
        /// <value>The maximum accepted response length.</value>
        public int MaxResponseLength { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepGetRequest"/> 
        /// class.
        /// </summary>
        /// <param name="request">The snep GET request information.</param>
        /// <param name="maxResponseLength">Maximum accepted response length.
        /// </param>
        public SnepGetRequest(INdefMessage request, int maxResponseLength)
            : this(Constants.DefaultSnepVersion, request, maxResponseLength)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepGetRequest"/> 
        /// class.
        /// </summary>
        /// <param name="version">The snep protocol version.</param>
        /// <param name="request">The snep GET request information.</param>
        /// <param name="maxResponseLength">Maximum accepted response length.
        /// </param>
        public SnepGetRequest(
            SnepVersion version, 
            INdefMessage request, 
            int maxResponseLength)
            : base(
                version, 
                SnepRequestCode.Get, 
                request, 
                maxResponseLength.ToByteArray())
        {
            if (maxResponseLength < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(maxResponseLength), 
                    "maxResponseLength must be positive.");
            }

            this.MaxResponseLength = maxResponseLength;
        }

        public const int ParseOffset = 4;
        public static int GetMaxResponseLength(Stream stream)
        {
            var buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            return IntExtensions.FromByteArray(buffer);
        }

        public static int GetMaxResponseLength(byte[] bytes)
        {
            return IntExtensions.FromByteArray(bytes);
        }


    }
}

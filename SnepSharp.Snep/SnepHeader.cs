//
//  SnepHeader.cs
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
    using System;
    using SnepSharp.Common;

    internal class SnepHeader
    {
        /// <summary>
        /// The snep header bytes.
        /// </summary>
        private byte[] snepHeader;

        /// <summary>
        /// Gets the command (Request/Response header field) for this message.
        /// </summary>
        /// <value>The command.</value>
        public byte Command { get; }

        /// <summary>
        /// Gets the snep protocol version.
        /// </summary>
        /// <value>The snep protocol version.</value>
        public SnepVersion Version { get; }

        /// <summary>
        /// Gets the length of the message content.
        /// </summary>
        /// <value>The length of the content.</value>
        public int ContentLength { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:SnepSharp.Snep.SnepHeader"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        /// <param name="command">Snep.</param>
        /// <param name="contentLength">Length of the message content.</param>
        public SnepHeader(SnepVersion version, byte command, int contentLength)
        {
            this.Version = version;
            this.Command = command;
            this.ContentLength = contentLength;

            this.snepHeader = new byte[Constants.SnepHeaderLength];
            this.snepHeader[0] = (byte)this.Version;
            this.snepHeader[1] = command;
            this.ContentLength.ToByteArray().CopyTo(this.snepHeader, 2);
        }

        /// <summary>
        /// Creates a new snep header from the specified byte array.
        /// Only uses the first bytes of the array to create the header, so the
        /// array is allowed to be larger.
        /// </summary>
        /// <returns>The created snep header.</returns>
        /// <param name="bytes">Byte array containing the snep header at the start.</param>
        public static SnepHeader FromBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            var version = (SnepVersion)bytes[0];
            var command = bytes[1];
            var contentLength = IntExtensions.FromByteArray(bytes, 2);

            return new SnepHeader(version, command, contentLength);
        }

        /// <summary>
        /// Creates a byte representation of the header.
        /// </summary>
        /// <returns>The header bytes.</returns>
        public byte[] AsBytes() => this.snepHeader;
    }
}

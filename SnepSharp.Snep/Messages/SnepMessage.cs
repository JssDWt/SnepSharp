//
//  SnepMessage.cs
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
    using System.IO;
    using SnepSharp.Common;
    using SnepSharp.Ndef;

    /// <summary>
    /// Snep message base class.
    /// </summary>
    internal abstract class SnepMessage
    {
        /// <summary>
        /// Gets the snep header of the message.
        /// </summary>
        /// <value>The header.</value>
        public SnepHeader Header { get; }

        /// <summary>
        /// Gets the length of the entire snep message.
        /// </summary>
        /// <value>The length of the message.</value>
        public int MessageLength 
            => this.Header.ContentLength + Constants.SnepHeaderLength;

        /// <summary>
        /// Gets the message content.
        /// </summary>
        /// <value>The information.</value>
        public INdefMessage Information { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepMessage"/> class, 
        /// with the default snep protocol version.
        /// </summary>
        /// <param name="command">The snep request or response.</param>
        /// <param name="message">The snep content.</param>
        protected SnepMessage(byte command, INdefMessage message)
            : this(Constants.DefaultSnepVersion, command, message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepMessage"/> class.
        /// </summary>
        /// <param name="version">The snep protocol version.</param>
        /// <param name="command">The snep request or response.</param>
        /// <param name="message">The snep content.</param>
        protected SnepMessage(
            SnepVersion version, 
            byte command, 
            INdefMessage message)
        {
            this.Information = message;
            this.Header = new SnepHeader(
                version, 
                command, 
                message?.Length ?? 0);
        }

        /// <summary>
        /// Creates a <see cref="Stream"/> representation of the message.
        /// </summary>
        /// <returns>The stream.</returns>
        public Stream AsStream()
        {
            Stream informationStream = this.InformationAsStream();
            if (informationStream == null)
            {
                return new MemoryStream(this.Header.AsBytes());
            }

            return new ByteHeaderStream(
                informationStream, 
                this.Header.AsBytes());
        }

        /// <summary>
        /// Creates a stream representation of the information field.
        /// Can be overridden in subclasses to append information.
        /// </summary>
        /// <returns>The information stream.</returns>
        protected virtual Stream InformationAsStream() 
            => this.Information?.AsStream();

        /// <summary>
        /// Creates a snep message from the specified SNEP header.
        /// </summary>
        /// <returns>The header, or null if a message could not be created from
        /// the header alone.</returns>
        /// <param name="header">Header.</param>
        public static SnepMessage FromHeader(SnepHeader header)
        {
            // request get, put
            // response success
            switch (header.Command)
            {
                case (byte)SnepRequestCode.Continue:
                    return new SnepContinueRequest(header.Version);
                case (byte)SnepRequestCode.Reject:
                    return new SnepRejectRequest(header.Version);
                case (byte)SnepResponseCode.BadRequest:
                    return new SnepBadRequestResponse(header.Version);
                case (byte)SnepResponseCode.Continue:
                    return new SnepContinueResponse(header.Version);
                case (byte)SnepResponseCode.ExcessData:
                    return new SnepExcessDataResponse(header.Version);
                case (byte)SnepResponseCode.NotFound:
                    return new SnepNotFoundResponse(header.Version);
                case (byte)SnepResponseCode.NotImplemented:
                    return new SnepNotImplementedResponse(header.Version);
                case (byte)SnepResponseCode.Reject:
                    return new SnepRejectResponse(header.Version);
                case (byte)SnepResponseCode.UnsupportedVersion:
                    return new SnepUnsupportedVersionResponse(header.Version);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Creates a <see cref="SnepMessage"/> subclass from the specified 
        /// <see cref="Stream"/>.
        /// </summary>
        /// <returns>The parsed message.</returns>
        /// <param name="header">Snep header indicating message type.</param>
        /// <param name="stream">Stream with pointer at the information part
        /// of the message.</param>
        /// <param name="ndefParser">Ndef parser.</param>
        public static SnepMessage FromStream(
            SnepHeader header, 
            Stream stream,
            INdefParser ndefParser)
        {
            var small = FromHeader(header);
            if (small != null) return small;

            // Handle all cases where an information field is involved.
            switch (header.Command)
            {
                case (byte)SnepRequestCode.Get:
                    int maxResponseLength =
                        SnepGetRequest.GetMaxResponseLength(stream);
                    return new SnepGetRequest(
                        header.Version,
                        ndefParser.ParseMessage(stream),
                        maxResponseLength);
                case (byte)SnepRequestCode.Put:
                    return new SnepPutRequest(
                        header.Version,
                        ndefParser.ParseMessage(stream));
                case (byte)SnepResponseCode.Success:
                    return new SnepSuccessResponse(
                        header.Version,
                        ndefParser.ParseMessage(stream));
                default:
                    throw new SnepException(
                        "Could not parse snep message, unknown type.");
            }
        }

        /// <summary>
        /// Creates a <see cref="SnepMessage"/> subclass from the specified 
        /// <see cref="byte[]"/>.
        /// </summary>
        /// <returns>The parsed message.</returns>
        /// <param name="message">Array containing the message, including 
        /// header. The first byte of the array should be the first header byte.
        /// </param>
        /// <param name="count">The amount of bytes the message contains.
        /// </param>
        /// <param name="ndefParser">Ndef parser.</param>
        public static SnepMessage Parse(
            byte[] message,
            int count, 
            INdefParser ndefParser)
        {
            if (count < Constants.SnepHeaderLength)
            {
                throw new SnepException("Message too small.");
            }

            var header = SnepHeader.FromBytes(message);
            var small = FromHeader(header);
            if (small != null) return small;

            // Handle all cases where an information field is involved.
            switch (header.Command)
            {
                case (byte)SnepRequestCode.Get:
                    int maxResponseLength =
                        SnepGetRequest.GetMaxResponseLength(message);
                    return new SnepGetRequest(
                        header.Version,
                        ndefParser.ParseMessage(
                            message,
                            Constants.SnepHeaderLength 
                                  + SnepGetRequest.ParseOffset,
                            count - Constants.SnepHeaderLength 
                                  - SnepGetRequest.ParseOffset),
                        maxResponseLength);
                case (byte)SnepRequestCode.Put:
                    return new SnepPutRequest(
                        header.Version,
                        ndefParser.ParseMessage(
                            message, 
                            Constants.SnepHeaderLength, 
                            count - Constants.SnepHeaderLength));
                case (byte)SnepResponseCode.Success:
                    return new SnepSuccessResponse(
                        header.Version,
                        ndefParser.ParseMessage(
                            message, 
                            Constants.SnepHeaderLength, 
                            count - Constants.SnepHeaderLength));
                default:
                    throw new SnepException(
                        "Could not parse snep message, unknown type.");
            }
        }
    }
}

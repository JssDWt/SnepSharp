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
    /// Represents a Protocol Data Unit sequence number in the header.
    /// </summary>
    public struct SequenceNumber
    {
        /// <summary>
        /// Gets the sequence number, as used in the PDU header.
        /// </summary>
        /// <value>The sequence.</value>
        public byte Sequence { get; }

        /// <summary>
        /// Gets the send sequence part of the sequence number.
        /// </summary>
        /// <value>The send sequence.</value>
        public byte SendSequence { get; }

        /// <summary>
        /// Gets the receive sequence part of the sequence number.
        /// </summary>
        /// <value>The receive sequence.</value>
        public byte ReceiveSequence { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.SequenceNumber"/> struct,
        /// When a PDU is received from the other party.
        /// </summary>
        /// <param name="sequence">The PDU sequence number.</param>
        public SequenceNumber(byte sequence)
        {
            this.Sequence = sequence;
            this.SendSequence = (byte)((sequence >> 4) & 0x0F);
            this.ReceiveSequence = (byte)(sequence & 0x0F);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.SequenceNumber"/> struct,
        /// Using the specified send and receive sequence to construct the sequence byte.
        /// </summary>
        /// <param name="send">Send sequence number.</param>
        /// <param name="receive">Receive sequence number.</param>
        public SequenceNumber(byte send, byte receive)
        {
            if (send < 0x00 || send > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(send), "sequence number should fit in four bits (0x00 to 0x0F).");
            }

            if (receive < 0x00 || receive > 0x0F)
            {
                throw new ArgumentOutOfRangeException(nameof(receive), "sequence number should fit in four bits (0x00 to 0x0F).");
            }

            this.SendSequence = send;
            this.ReceiveSequence = receive;

            this.Sequence = (byte)((send << 4) | receive);
        }
    }
}

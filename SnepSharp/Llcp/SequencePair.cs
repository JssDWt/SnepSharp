//
//  SequencePair.cs
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
    /// <summary>
    /// Represents a Protocol Data Unit sequence number in the header.
    /// </summary>
    public struct SequencePair
    {
        /// <summary>
        /// Sequence number reperesenting zero for both the send and receive 
        /// part.
        /// </summary>
        public static readonly SequencePair Zero = new SequencePair(
            (SequenceNumber)0,
            (SequenceNumber)0);

        /// <summary>
        /// Gets the sequence number, as used in the PDU header.
        /// </summary>
        /// <value>The sequence.</value>
        public byte Sequence { get; }

        /// <summary>
        /// Gets the send sequence part of the sequence number.
        /// </summary>
        /// <value>The send sequence.</value>
        public SequenceNumber SendSequence { get; }

        /// <summary>
        /// Gets the receive sequence part of the sequence number.
        /// </summary>
        /// <value>The receive sequence.</value>
        public SequenceNumber ReceiveSequence { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.SequenceNumber"/> struct,
        /// When a PDU is received from the other party.
        /// </summary>
        /// <param name="sequence">The PDU sequence number.</param>
        public SequencePair(byte sequence)
        {
            this.Sequence = sequence;
            this.SendSequence = (SequenceNumber)((sequence >> 4) & 0x0F);
            this.ReceiveSequence = (SequenceNumber)(sequence & 0x0F);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:Llcp.SequenceNumber"/> struct,
        /// Using the specified send and receive sequence to construct the sequence byte.
        /// </summary>
        /// <param name="send">Send sequence number.</param>
        /// <param name="receive">Receive sequence number.</param>
        public SequencePair(SequenceNumber send, SequenceNumber receive)
        {
            this.SendSequence = send;
            this.ReceiveSequence = receive;

            this.Sequence = (byte)((send << 4) | receive);
        }
    }
}

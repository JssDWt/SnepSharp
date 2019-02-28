//
//  MacMapping.cs
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

namespace SnepSharp.Mac
{
    using System;
    using SnepSharp.Llcp.Pdus;
    using SnepSharp.Nfc;

    /// <summary>
    /// Data exchange protocol.
    /// </summary>
    /// <remarks>
    /// Target device RWT should be sufficiently larger than the llcp LTO, to 
    /// allow one or more MAC recovry cycles.
    /// </remarks>
    internal abstract class MacMapping
    {
        public CommunicationManager Manager { get; }

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="MacMapping"/> is initiator.
        /// </summary>
        /// <value><c>true</c> if is initiator; otherwise, <c>false</c>.</value>
        public bool IsInitiator { get; }

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="MacMapping"/> should use device identifier 
        /// (DID).
        /// </summary>
        /// <value><c>true</c> if use device identifier; otherwise, 
        /// <c>false</c>.</value>
        public bool UseDeviceIdentifier { get; } = false;

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="MacMapping"/> should use node address (NAD).
        /// </summary>
        /// <value><c>true</c> if use node address; otherwise, <c>false</c>.
        /// </value>
        public bool UseNodeAddress { get; } = false;

        /// <summary>
        /// Gets a value indicating whether this 
        /// <see cref="MacMapping"/> should use waiting time 
        /// extensions (WTX).
        /// </summary>
        /// <value><c>true</c> if use waiting time extensions; otherwise, 
        /// <c>false</c>.</value>
        public bool UseWaitingTimeExtensions { get; } = false;

        /// <summary>
        /// Gets the value to use for length reduction initiator (LRi).
        /// </summary>
        public bool LengthReductionInitiator { get; } = true;

        /// <summary>
        /// Gets the value to use for length reduction target (LRt).
        /// </summary>
        public bool LengthReductionTarget { get; } = true;

        /// <summary>
        /// Gets the response waiting time (RWT).
        /// </summary>
        /// <value>The response waiting time.</value>
        public TimeSpan ResponseWaitingTime { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MacMapping"/> 
        /// class.
        /// </summary>
        /// <param name="isInitiator">If set to <c>true</c> is initiator.
        /// </param>
        protected MacMapping(CommunicationManager manager, bool isInitiator)
        {
            this.Manager = manager;
            this.IsInitiator = isInitiator;
        }

        /// <summary>
        /// Activates the connection.
        /// </summary>
        /// <returns>The activate.</returns>
        /// <param name="paxPdu">Pax pdu.</param>
        public abstract ParameterExchangeUnit Activate(
            ParameterExchangeUnit paxPdu);
        // TODO: Provide additional options?
        // TODO: Should throw if the response does not contain the llcp magic number.

        public abstract void Deactivate();

        public ProtocolDataUnit Exchange(ProtocolDataUnit sendPdu)
        {
            throw new NotImplementedException();
        }
    }
}

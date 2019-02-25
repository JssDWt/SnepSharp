//
//  ReceiveWindowSizeParameter.cs
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
    using System;

    /// <summary>
    /// The receive window size (RW) is a data link connection parameter that 
    /// MAY be transmitted with a CONNECT or a CC PDU and applies to the sender 
    /// of the CONNECT or CC PDU.
    /// </summary>
    internal class ReceiveWindowSizeParameter : Parameter
    {
        /// <summary>
        /// Gets the receive window size.
        /// </summary>
        /// <remarks>A receive window size of zero indicates that the local LLC 
        /// will not accept I PDUs on that data link connection. A receive 
        /// window size of one indicates that the local LLC will acknowledge 
        /// every I PDU before accepting additional I PDUs.</remarks>
        /// <value>The size of the window.</value>
        public int WindowSize { get; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ReceiveWindowSizeParameter"/> class.
        /// </summary>
        /// <param name="windowSize">Window size.</param>
        public ReceiveWindowSizeParameter(int windowSize)
            : base(
                  ParameterType.ReceiveWindowSize, 
                  new byte[] { (byte)windowSize })
        {
            if (windowSize < 0 || windowSize > 15)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(windowSize), 
                    "windowSize must be between 0 and 15 inclusive.");
            }

            this.WindowSize = windowSize;
        }
    }
}

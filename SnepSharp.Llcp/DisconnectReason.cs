//
//  DisconnectReason.cs
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
    /// Disconnect reason codes.
    /// </summary>
    internal enum DisconnectReason
    {
        /// <summary>
        /// SHALL indicate that the LLC has received a DISC PDU and is now 
        /// logically disconnected from the data link connection.
        /// </summary>
        DisconnectReceived = 0x00,

        /// <summary>
        /// SHALL indicate that the LLC has received a connection-oriented PDU 
        /// but the target service access point has no active connection.
        /// </summary>
        NoActiveConnection = 0x01,

        /// <summary>
        /// SHALL indicate that the remote LLC has received a CONNECT PDU and 
        /// there is no service bound to the specified target service access 
        /// point.
        /// </summary>
        NoServiceBound = 0x02,

        /// <summary>
        /// SHALL indicate that the remote LLC has processed a CONNECT PDU and 
        /// the request to connect was rejected by the service layer.
        /// </summary>
        RejectedByServiceLayer = 0x03,

        /// <summary>
        /// SHALL indicate that the LLC will permanently not accept any CONNECT 
        /// PDUs with the same target service access point address.
        /// </summary>
        PermanentInvalidAddress = 0x10,

        /// <summary>
        /// SHALL indicate that the LLC will permanently not accept any CONNECT 
        /// PDUs with any target service access point address.
        /// </summary>
        PermanentlyUnavailable = 0x11,

        /// <summary>
        /// SHALL indicate that the LLC will temporarily not accept any CONNECT 
        /// PDUs with the specified target service access point address.
        /// </summary>
        TemporaryInvalidAddress = 0x20,

        /// <summary>
        /// SHALL indicate that the LLC will temporarily not accept any CONNECT 
        /// PDUs with any target service access point address.
        /// </summary>
        TemporarylyUnavailable = 0x21
    }
}

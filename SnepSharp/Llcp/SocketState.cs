//
//  SocketState.cs
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
    public enum SocketState : byte
    {
        /// <summary>
        /// Indicates the socket is shutdown and cannot be reopened.
        /// </summary>
        Shutdown,

        /// <summary>
        /// Indicates the socket has not yet been opened.
        /// </summary>
        Closed,

        /// <summary>
        /// Indicates the socket is listening for incoming connections.
        /// </summary>
        Listening,

        /// <summary>
        /// Indicates the socket is trying to connect to a peer.
        /// </summary>
        Connecting,

        /// <summary>
        /// Indicates the socket is connected to a peer.
        /// </summary>
        Connected,

        /// <summary>
        /// 
        /// </summary>
        Established,

        /// <summary>
        /// Indicates the socket is in the process of disconnecting from the 
        /// peer.
        /// </summary>
        Disconnecting,

        /// <summary>
        /// Indicates the socket is closing to be shutdown shortly.
        /// </summary>
        Closing
    }
}

//
//  SnepPutRequest.cs
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
    using SnepSharp.Ndef;

    /// <summary>
    /// Snep put request. To put a resource to the server.
    /// </summary>
    internal class SnepPutRequest : SnepRequest
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepPutRequest"/> class.
        /// </summary>
        /// <param name="content">Content to put to the server.</param>
        public SnepPutRequest(INdefMessage content)
            : this(Constants.DefaultSnepVersion, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepPutRequest"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        /// <param name="content">Content to put to the server.</param>
        public SnepPutRequest(SnepVersion version, INdefMessage content)
            : base(version, SnepRequestCode.Put, content)
        {
            if (content == null)
            {
                throw new ArgumentNullException(
                    nameof(content), 
                    "Snep put requests MUST have information");
            }
        }
    }
}

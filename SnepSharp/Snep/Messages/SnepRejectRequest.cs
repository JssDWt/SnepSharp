﻿//
//  SnepRejectRequest.cs
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
    /// <summary>
    /// Snep reject request. Indicates that the server response is rejected.
    /// </summary>
    internal class SnepRejectRequest : SnepRequest
    {
        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepRejectRequest"/> class.
        /// </summary>
        public SnepRejectRequest()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="SnepRejectRequest"/> class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepRejectRequest(SnepVersion version)
            : base(version, SnepRequestCode.Reject, null)
        {
        }
    }
}
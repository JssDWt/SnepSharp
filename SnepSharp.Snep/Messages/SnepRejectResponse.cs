//
//  SnepRejectResponse.cs
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
    /// Indicates the snep server is unable to receive remaining fragments.
    /// Returned after receipt of the first fragment of a fragmented snep 
    /// message.
    /// </summary>
    internal class SnepRejectResponse : SnepResponse
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SnepRejectResponse"/> 
        /// class.
        /// </summary>
        public SnepRejectResponse()
            : this(Constants.DefaultSnepVersion)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepRejectResponse"/> 
        /// class.
        /// </summary>
        /// <param name="version">Snep protocol version.</param>
        public SnepRejectResponse(SnepVersion version)
            : base(version, SnepResponseCode.Reject, null)
        {
        }
    }
}

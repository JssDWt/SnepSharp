//
//  SnepResponse.cs
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
    using SnepSharp.Ndef;

    /// <summary>
    /// Snep response message.
    /// </summary>
    internal abstract class SnepResponse : SnepMessage
    {
        /// <summary>
        /// Gets the response status code.
        /// </summary>
        /// <value>The response status code.</value>
        public SnepResponseCode Response { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepResponse"/> class.
        /// </summary>
        /// <param name="response">The snep response code.</param>
        /// <param name="content">The response content.</param>
        protected SnepResponse(SnepResponseCode response, INdefMessage content)
            : this(Constants.DefaultSnepVersion, response, content)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepResponse"/> class.
        /// </summary>
        /// <param name="version">The snep protocol version.</param>
        /// <param name="response">The snep response code.</param>
        /// <param name="content">The response content.</param>
        protected SnepResponse(
            SnepVersion version, 
            SnepResponseCode response, 
            INdefMessage content)
            : base(version, (byte)response, content)
        {
            this.Response = response;
        }
    }
}

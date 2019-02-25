//
//  SnepRequest.cs
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
    using System.IO;
    using SnepSharp.Ndef;
    using SnepSharp.Common;

    /// <summary>
    /// Snep request message.
    /// </summary>
    internal abstract class SnepRequest : SnepMessage
    {
        /// <summary>
        /// Prefix for the information field.
        /// </summary>
        private readonly byte[] informationPrefix;

        /// <summary>
        /// Gets the request operation/command.
        /// </summary>
        /// <value>The request operation/command.</value>
        public SnepRequestCode Request { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepRequest"/> class.
        /// </summary>
        /// <param name="request">The request code.</param>
        /// <param name="content">The request content.</param>
        /// <param name="informationPrefix">An optional prefix field to the 
        /// information field.</param>
        protected SnepRequest(
            SnepRequestCode request, 
            INdefMessage content, 
            byte[] informationPrefix = null)
            : this(
                  Constants.DefaultSnepVersion, 
                  request, 
                  content, 
                  informationPrefix)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SnepRequest"/> class.
        /// </summary>
        /// <param name="version">The snep protocol version.</param>
        /// <param name="request">The request code.</param>
        /// <param name="content">The request content.</param>
        /// <param name="informationPrefix">An optional prefix field to the 
        /// information field.</param>
        protected SnepRequest(
            SnepVersion version, 
            SnepRequestCode request, 
            INdefMessage content, 
            byte[] informationPrefix = null)
            :base(version, (byte)request, content)
        {
            this.Request = request;
            this.informationPrefix = informationPrefix;
        }

        /// <summary>
        /// Appends the information prefix to the base information stream.
        /// </summary>
        /// <returns>The stream.</returns>
        protected override Stream InformationAsStream()
        {
            Stream baseStream = base.InformationAsStream();

            if (this.informationPrefix == null || baseStream == null)
            {
                return baseStream;
            }

            return new ByteHeaderStream(
                baseStream, 
                this.informationPrefix, 
                disposeInner: true);
        }
    }
}

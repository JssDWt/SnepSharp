//
//  VersionParameter.cs
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
    /// <summary>
    /// The version number (VERSION) parameter is a configuration parameter that 
    /// MUST be transmitted during the link activation. It SHALL denote the 
    /// major and minor release levels of the LLCP specification implemented by 
    /// the sending LLC.
    /// </summary>
    internal class VersionParameter : Parameter
    {
        /// <summary>
        /// Gets the LLCP version.
        /// </summary>
        /// <value>The LLCP version.</value>
        public LlcpVersion Version { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionParameter"/> 
        /// class.
        /// </summary>
        /// <param name="version">The LLCP version.</param>
        public VersionParameter(LlcpVersion version)
            : base(ParameterType.Version, new byte[] { version.Version })
        {
            this.Version = version;
        }
    }
}

//
//  ServiceDiscoveryPoint.cs
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
    using System.Collections.Generic;
    using SnepSharp.Llcp.Pdus;

    /// <summary>
    /// A service dicovery point, for dicovering the services available on the
    /// <see cref="LogicalLinkControl"/>.
    /// </summary>
    internal class ServiceDiscoveryPoint : ServiceAccessPoint
    {
        /// <summary>
        /// Gets the disconnected mode pdus sent to the remote party.
        /// </summary>
        /// <value>The disconnected mode pdus.</value>
        public ICollection<DisconnectedModeUnit> DisconnectedModePdus { get; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ServiceDiscoveryPoint"/> class.
        /// </summary>
        /// <param name="llc">Logical link control.</param>
        public ServiceDiscoveryPoint(LogicalLinkControl llc)
            : base((LinkAddress)1, llc)
        {
            this.DisconnectedModePdus = new List<DisconnectedModeUnit>();
        }
    }
}

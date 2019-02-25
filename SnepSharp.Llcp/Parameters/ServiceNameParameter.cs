//
//  ServiceNameParameter.cs
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
    /// The service name (SN) parameter MAY be transmitted with a CONNECT PDU to
    /// the well- known destination service access point address 01h and SHALL 
    /// then indicate that the sending LLC intends to establish a data link 
    /// connection with the named service registered in the remote service 
    /// environment.
    /// </summary>
    internal class ServiceNameParameter : Parameter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceNameParameter"/> 
        /// class.
        /// </summary>
        /// <param name="serviceName">Service name.</param>
        public ServiceNameParameter(string serviceName)
            : base(ParameterType.ServiceName, AsBytes(serviceName))
        {
        }

        /// <summary>
        /// Creates a byte representation of the servicename.
        /// </summary>
        /// <returns>The bytes.</returns>
        /// <param name="serviceName">Service name.</param>
        private static byte[] AsBytes(string serviceName)
        {
            if (string.IsNullOrEmpty(serviceName))
            {
                throw new ArgumentException(
                    "serviceName cannot be null or empty", 
                    nameof(serviceName));
            }

            return Constants.DefaultEncoding.GetBytes(serviceName);
        }
    }
}

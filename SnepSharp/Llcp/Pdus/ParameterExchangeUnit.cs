//
//  ParameterExchangeUnit.cs
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

namespace SnepSharp.Llcp.Pdus
{
    using System;
    using System.Collections.Generic;
    using SnepSharp.Llcp.Parameters;

    /// <summary>
    /// Parameter exchange (PAX) unit. SHALL be used to exchange parameters 
    /// concerning the LLCP Link configuration.
    /// </summary>
    internal class ParameterExchangeUnit : ProtocolDataUnit
    {
        public LlcpVersion Version { get; }
        public int MaximumInformationUnit { get; } 
        public ICollection<LinkAddress> WellknownServices { get; }
        public int LinkTimeout { get; }
        LinkServiceClass ServiceClass { get; }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <value>The parameters.</value>
        public ParameterList Parameters { get; }

        /// <summary>
        /// Initializes a new instance of the 
        /// <see cref="ParameterExchangeUnit"/> class.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        public ParameterExchangeUnit(
            LlcpVersion version,
            int miu = Constants.MaximumInformationUnit,
            ICollection<LinkAddress> wellknownServices = null,
            int linkTimeout = Constants.DefaultTimeout,
            LinkServiceClass serviceClass = LinkServiceClass.Unknown)
            : base(
                  DataLink.Empty, 
                  ProtocolDataUnitType.ParameterExchange, 
                  null, 
                  ToBytes(
                    version,
                    miu,
                    wellknownServices,
                    linkTimeout,
                    serviceClass))
        {
            this.Version = version;
            this.MaximumInformationUnit = MaximumInformationUnit;
            this.WellknownServices = wellknownServices 
                ?? new List<LinkAddress>();
            this.LinkTimeout = linkTimeout;
            this.ServiceClass = serviceClass;
        }

        /// <summary>
        /// Converts the specified parameters to bytes.
        /// </summary>
        /// <returns>The byte representation of the parameters.</returns>
        /// <param name="parameters">Parameters.</param>
        private static byte[] ToBytes(
            LlcpVersion version,
            int miu,
            ICollection<LinkAddress> wellknownServices,
            int linkTimeout,
            LinkServiceClass serviceClass)
        {
            var parameters = new ParameterList
            {
                new VersionParameter(version)
            };

            if (miu != Constants.MaximumInformationUnit)
            {
                parameters.Add(new MiuxParameter(miu));
            }

            if (wellknownServices != null && wellknownServices.Count > 0)
            {
                parameters.Add(new WksParameter(wellknownServices));
            }

            if (linkTimeout != Constants.DefaultTimeout)
            {
                parameters.Add(new LinkTimeoutParameter(linkTimeout));
            }

            if (serviceClass != LinkServiceClass.Unknown)
            {
                parameters.Add(new OptionParameter(serviceClass));
            }

            return parameters.ToBytes();
        }

        /// <summary>
        /// Parses the specified bytes into parameters.
        /// </summary>
        /// <returns>The parameters.</returns>
        /// <param name="data">bytes to parse.</param>
        private static ParameterList FromBytes(byte[] data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var list = ParameterList.FromBytes(data);
            if (list.Count == 0)
            {
                throw new ArgumentException(
                    "At least one parameter should be supplied.", 
                    nameof(data));
            }

            return list;
        }
    }
}

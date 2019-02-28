//
//  ParameterType.cs
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
    /// LLCP Parameter type codes.
    /// </summary>
    public enum ParameterType
    {
        /// <summary>
        /// The version number (VERSION) parameter is a configuration parameter 
        /// that MUST be transmitted during the link activation. It SHALL denote 
        /// the major and minor release levels of the LLCP specification 
        /// implemented by the sending LLC.
        /// </summary>
        Version = 0x01,

        /// <summary>
        /// The maximum information unit (MIU) is the maximum number of octets 
        /// in the information field of an LLC PDU that the local LLC is able 
        /// to receive. The default MIU is 128.
        /// </summary>
        MiuxExtension = 0x02,

        /// <summary>
        /// The well-known service (WKS) parameter is a configuration parameter 
        /// that SHALL denote the binding of service listeners to well-known 
        /// service access point addresses and therefore the willingness of the 
        /// sender of the WKS parameter to accept PDUs on those SAPs.
        /// </summary>
        WellKnownServiceList = 0x03,

        /// <summary>
        /// The link timeout (LTO) parameter is a configuration parameter that 
        /// specifies the local link timeout interval guarantee.
        /// </summary>
        LinkTimeout = 0x04,

        /// <summary>
        /// A receive window size of zero indicates that the local LLC will not 
        /// accept I PDUs on that data link connection. A receive window size of 
        /// one indicates that the local LLC will acknowledge every I PDU before 
        /// accepting additional I PDUs.
        /// </summary>
        ReceiveWindowSize = 0x05,

        /// <summary>
        /// The service name (SN) parameter MAY be transmitted with a CONNECT 
        /// PDU to the well- known destination service access point address 01h 
        /// and SHALL then indicate that the sending LLC intends to establish a 
        /// data link connection with the named service registered in the remote 
        /// service environment.
        /// </summary>
        ServiceName = 0x06,

        /// <summary>
        /// The Option parameter communicates the link service class and the set 
        /// of options supported by the sending LLC.
        /// </summary>
        Option = 0x07
    }
}

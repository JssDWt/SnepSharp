//
//  ProtocolDataUnitType.cs
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
    public enum ProtocolDataUnitType
    {
        Symmetry = 0x00,
        ParameterExchange = 0x01,
        AggregatedFrame = 0x02,
        UnnumberedInformation = 0x03,
        Connect = 0x04,
        Disconnect = 0x05,
        ConnectionComplete = 0x06,
        DisconnectedMode = 0x07,
        FrameReject = 0x08,
        Reserved1 = 0x09,
        Reserved2 = 0x0A,
        Reserved3 = 0x0B,
        Information = 0x0C,
        ReceiveReady = 0x0D,
        ReceiveNotReady = 0x0E,
        Reserved4 = 0x0F
    }
}

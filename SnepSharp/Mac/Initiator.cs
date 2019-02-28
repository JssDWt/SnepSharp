//
//  Initiator.cs
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
using System;
using SnepSharp.Llcp.Pdus;
using SnepSharp.Nfc;

namespace SnepSharp.Mac
{
    internal class Initiator : MacMapping
    {
        public Initiator(CommunicationManager manager)
            : base(manager, isInitiator: true)
        {
        }

        public override ParameterExchangeUnit Activate(
            ParameterExchangeUnit paxPdu) => throw new NotImplementedException();

        public override void Deactivate() => throw new NotImplementedException();
    }
}

//
//  FlowTests.cs
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

namespace Tests.DataLinkConnection
{
    using System;
    using System.Threading.Tasks;
    using Moq;
    using SnepSharp.Llcp;
    using SnepSharp.Llcp.Pdus;
    using Xunit;

    public class StateTests
    {
        [Fact]
        public void Initialized_State_Closed()
        {
            var llc = new Mock<LogicalLinkControl>();
            ISocket socket = new DataLinkConnection(llc.Object);
            Assert.Equal(SocketState.Closed, socket.State);
        }

        [Fact]
        public async Task Connect_Address_State_Established()
        {
            var llc = new Mock<LogicalLinkControl>();

            ISocket socket = new DataLinkConnection(llc.Object);
            llc.Setup(l => l.Bind(It.IsAny<ISocket>())).Verifiable();
            ILlcDispatch dispatch = (ILlcDispatch)socket;
            Task connect = socket.Connect(new LinkAddress(4));
            await Task.Delay(10);
            ProtocolDataUnit pdu = dispatch.DequeueForSend(1000);
            Assert.NotNull(pdu);
            var cc = new ConnectionCompleteUnit(DataLink.Empty);
            dispatch.EnqueueReceived(cc);
            await connect;
            Assert.Equal(SocketState.Established, socket.State);
        }
    }
}

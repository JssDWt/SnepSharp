//
//  Socket.cs
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
    using System;
    using System.Collections.Generic;
    using SnepSharp.Llcp.Pdus;

    public class Socket : IDisposable
    {
        public int SendMiu { get; internal set; }
        public SocketState State { get; }
        public int MaximumInformationUnit { get; }
        public LinkAddress? Address { get; }
        public LinkAddress? Peer { get; }

        private Queue<ProtocolDataUnit> receiveQueue
            = new Queue<ProtocolDataUnit>();

        private LogicalLinkControl llc;
        internal Socket(LogicalLinkControl llc)
        {
            this.llc = llc;
        }

        public void Bind(LinkAddress address) => this.llc.Bind(this, address);

        public void Bind(string serviceName) 
            => this.llc.Bind(this, serviceName);

        public void Bind() => this.llc.Bind(this);

        public void Unbind()
        {
            throw new NotImplementedException();
        }

        public void Connect(LinkAddress destination)
        {
            if (!this.Address.HasValue)
            {
                this.Bind();
            }

            // TODO: Logic to actually connect.

            if (this.SendMiu > this.llc.SendMiu)
            {
                this.SendMiu = this.llc.SendMiu;
            }

            throw new NotImplementedException();
        }

        public void Connect(string serviceName)
        {
            if (!this.Address.HasValue)
            {
                this.Bind();
            }

            // TODO: Logic to actually connect.

            if (this.SendMiu > this.llc.SendMiu)
            {
                this.SendMiu = this.llc.SendMiu;
            }

            throw new NotImplementedException();
        }

        public void Listen(int receiveBuffer)
        {
            if (receiveBuffer < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(receiveBuffer),
                    "Cannot be negative.");
            }

            receiveBuffer = Math.Min(receiveBuffer, 16);
            if (!this.Address.HasValue)
            {
                this.Bind();
            }

            // TODO: actual listen logic.

            throw new NotImplementedException();
        }

        public bool Send(byte[] message, int count)
            => this.Send(message, 0, count);

        public bool Send(byte[] message, int offset, int count)
        {
            // TODO: convert into a pdu.
            // TODO: throw if not connected.
            // TODO: send message.
            throw new NotImplementedException();
        }


        public int Receive(byte[] buffer)
        {
            // TODO: CHECK whether sap is connected.
            // TODO: receive data.
            throw new NotImplementedException();
        }

        public bool Poll(/* TODO: accept type parameter (ack, send, receive) */)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        internal void Enqueue(ProtocolDataUnit receivePdu)
        {
            this.receiveQueue.Enqueue(receivePdu);
        }

        internal ProtocolDataUnit Dequeue(int maxInformationUnit)
        {
            throw new NotImplementedException();
        }

        internal ProtocolDataUnit SendAcknowledgement()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose() => this.Close();
    }
}

//
//  LlcpSocket.cs
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

    public class LlcpSocket : IDisposable
    {
        public SocketState State { get; }
        public int MaximumInformationUnit { get;  }
        public LinkAddress? Address { get; }
        public LinkAddress? Peer { get; }

        private Queue<ProtocolDataUnit> receiveQueue
            = new Queue<ProtocolDataUnit>();

        public void Bind(LinkAddress address)
        {
            throw new NotImplementedException();
        }

        public void Unbind()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }


        public void ConnectToService(string serviceName)
        {
            throw new NotImplementedException();
        }

        internal void Enqueue(ProtocolDataUnit receivePdu)
        {
            this.receiveQueue.Enqueue(receivePdu);
        }

        internal ProtocolDataUnit Dequeue(int maximumInformationUnit)
        {
            // should return null is not found.
            throw new NotImplementedException();
        }

        internal ProtocolDataUnit SendAcknowledgement()
        {
            // could return null.
            throw new NotImplementedException();
        }

        public void Send(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }

            this.Send(buffer, buffer.Length);
        }

        public void Send(byte[] buffer, int length)
        {
            throw new NotImplementedException();
        }

        public int Receive(byte[] buffer)
        {
            throw new NotImplementedException();
        }

        public void Dispose() => this.Close();
    }
}

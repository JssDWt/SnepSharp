//
//  ServiceAccessPoint.cs
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
    using System.Linq;
    using SnepSharp.Llcp.Pdus;

    internal class ServiceAccessPoint : IDisposable
    {
        private readonly List<LlcpSocket> sockets = new List<LlcpSocket>();
        private readonly Queue<ProtocolDataUnit> sendQueue 
            = new Queue<ProtocolDataUnit>();

        protected LogicalLinkControl llc;

        // TODO: Determine whether addinional locking from this class is required.
        public ServiceAccessPoint(LinkAddress address, LogicalLinkControl llc)
        {
            this.Address = address;
            this.llc = llc ?? throw new ArgumentNullException(nameof(llc));
        }

        public LinkAddress Address { get; }

        public void AddSocket(LlcpSocket socket)
        {
            socket.Bind(this.Address);
            this.sockets.Add(socket);
        }

        public void RemoveSocket(LlcpSocket socket)
        {
            if (socket.Address != this.Address)
            {
                throw new ArgumentException("Wrong socket address.");
            }

            socket.Close();

            this.sockets.Remove(socket);

            if (this.sockets.Count == 0)
            {
                llc.ServiceAccessPoints[this.Address] = null;
            }
        }

        public void Send(ProtocolDataUnit sendPdu)
        {
            this.sendQueue.Enqueue(sendPdu);
        }

        public void Enqueue(ProtocolDataUnit receivePdu)
        {
            if (receivePdu is ConnectUnit)
            {
                var listeningSocket = this.sockets.FirstOrDefault(
                    s => s.State == SocketState.Listening);

                if (listeningSocket == null)
                {
                    // No listening socket, send disconnect.
                    var disconnect = new DisconnectedModeUnit(
                        receivePdu.DataLinkConnection,
                        DisconnectReason.NoActiveConnection);
                    this.Send(disconnect);
                }
                else
                {
                    listeningSocket.Enqueue(receivePdu);
                }
            }
            else
            {
                var socket = this.sockets.FirstOrDefault(
                    s => s.Peer == receivePdu.DataLinkConnection.Source
                        || !s.Peer.HasValue);

                if (socket == null)
                {
                    switch (receivePdu.Type)
                    {
                        case ProtocolDataUnitType.Connect:
                        case ProtocolDataUnitType.Disconnect:
                        case ProtocolDataUnitType.ConnectionComplete:
                        case ProtocolDataUnitType.DisconnectedMode:
                        case ProtocolDataUnitType.FrameReject:
                        case ProtocolDataUnitType.Information:
                        case ProtocolDataUnitType.ReceiveReady:
                        case ProtocolDataUnitType.ReceiveNotReady:
                            var disconnect = new DisconnectedModeUnit(
                                receivePdu.DataLinkConnection,
                                DisconnectReason.NoActiveConnection);
                            this.Send(disconnect);
                            break;
                    }
                }
                else
                {
                    socket.Enqueue(receivePdu);
                }
            }
        }

        public ProtocolDataUnit Dequeue(int maximumInformationUnit)
        {
            foreach (var socket in this.sockets)
            {
                var sendPdu = socket.Dequeue(maximumInformationUnit);
                if (sendPdu != null)
                {
                    return sendPdu;
                }
            }

            if (this.sendQueue.Count > 0)
            {
                return this.sendQueue.Dequeue();
            }

            return null;
        }

        public ProtocolDataUnit SendAcknowledgement()
        {
            foreach(var socket in this.sockets)
            {
                var sendPdu = socket.SendAcknowledgement();
                if (sendPdu != null)
                {
                    return sendPdu;
                }
            }

            return null;
        }

        public void Close()
        {
            foreach(var socket in this.sockets)
            {
                socket.Unbind();
                socket.Close();
            }

            this.sockets.Clear();
        }

        public void Dispose()
        {
            this.Close();
        }
    }
}

//
//  ServerSocket.cs
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
    using System.Threading;

    public class ServerSocket
    {
        private LogicalLinkControl llc;

        internal ServerSocket(LogicalLinkControl llc)
        {
        }

        public Socket Accept() => this.Accept(new CancellationToken(false));

        public Socket Accept(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                // TODO: actual socket creation logic.
                Socket clientSocket = null;
                throw new NotImplementedException();
                if (clientSocket == null) continue;

                this.llc.AddToSap(clientSocket);

                if (clientSocket.SendMiu > this.llc.SendMiu)
                {
                    clientSocket.SendMiu = this.llc.SendMiu;
                }

                return clientSocket;
            }

            return null;
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose() => this.Close();
    }
}

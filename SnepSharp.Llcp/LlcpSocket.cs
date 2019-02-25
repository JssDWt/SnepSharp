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

    public class LlcpSocket : IDisposable
    {
        public int MaximumInformationUnit { get;  }
        public int RemoteMaximumInformationUnit { get; }
        public int RemoteReceiveWindowSize { get; }
        public int LocalSap { get; }
        public int LocalMaximumInformationUnit { get; }
        public int LocalReceiveWindowSize { get; }

        public void ConnectToSap(int sap)
        {
            throw new NotImplementedException();
        }

        public void ConnectToService(string serviceName)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
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

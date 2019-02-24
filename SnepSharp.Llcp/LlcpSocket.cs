using System;
namespace SnepSharp.Llcp
{
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

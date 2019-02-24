using System;
using System.Threading;

namespace SnepSharp.Llcp
{
    public class LlcpServerSocket : IDisposable
    {
        public LlcpSocket Accept()
        {
            throw new NotImplementedException();
        }

        public LlcpSocket Accept(CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose() => this.Close();
    }
}

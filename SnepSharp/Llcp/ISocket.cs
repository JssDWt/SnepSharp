namespace SnepSharp.Llcp
{
    using System;
    using System.Threading;

    public interface ISocket : IDisposable
    {
        int RemoteReceiveWindowSize { get; }
        int LocalReceiveWindowSize { get; }
        int SendBufferSize { get; }
        int ReceiveBufferSize { get; }
        bool IsBound { get; }
        bool IsConnected { get; }
        int RemoteMiu { get; }
        SocketState State { get; }
        int LocalMiu { get; }
        LinkAddress? Address { get; }
        LinkAddress? Peer { get; }

        void Close();
        byte[] Receive();
        byte[] Receive(CancellationToken token);
        void Send(byte[] message, int count);
        void Send(byte[] message, int count, CancellationToken token);
        void Send(byte[] message, int offset, int count);
        void Send(byte[] message, int offset, int count, CancellationToken token);
    }
}
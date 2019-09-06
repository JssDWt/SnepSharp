namespace SnepSharp.Llcp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

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

        Task Connect(LinkAddress destination);
        Task Connect(LinkAddress destination, CancellationToken token);
        Task Connect(string serviceName);
        Task Connect(string serviceName, CancellationToken token);
        void Close();
        Task<byte[]> Receive();
        Task<byte[]> Receive(CancellationToken token);
        Task Send(byte[] message, int count);
        Task Send(byte[] message, int count, CancellationToken token);
        Task Send(byte[] message, int offset, int count);
        Task Send(byte[] message, int offset, int count, CancellationToken token);
    }
}
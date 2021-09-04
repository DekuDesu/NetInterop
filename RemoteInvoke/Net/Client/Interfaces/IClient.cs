using System;
using System.Net.Sockets;
#nullable enable
namespace RemoteInvoke.Net.Client
{
    public interface IClient<T> : IDisposable where T : IDisposable
    {
        T? Client { get; }
        bool Connected { get; }
        bool Available { get; }

        void Disconnect();
        void Disconnect(int timeout);
        bool TryConnect(out T stream);
    }
}
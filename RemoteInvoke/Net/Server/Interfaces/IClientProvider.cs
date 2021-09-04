using System.Net.Sockets;

namespace RemoteInvoke.Net.Server
{
    /// <summary>
    /// Responsible for actually providing clients to an <see cref="IServer{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IClientProvider<T>
    {
        bool Pending();
        T AcceptClient();
        void SetPort(int port);
        void Start();
        void Stop();
    }
}
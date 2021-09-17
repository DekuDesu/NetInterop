using System.Net.Sockets;

namespace NetInterop.Transport.Core.Abstractions
{
    /// <summary>
    /// Responsible for actually providing clients to an <see cref="IServer{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IConnectionProviderOld<T>
    {
        bool Pending();
        T AcceptClient();
        void SetPort(int port);
        void Start();
        void Stop();
    }
}
using NetInterop.Transport.Core.Abstractions.Connections;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NetInterop.Abstractions
{
    public interface IServer
    {
        IConnection Connection { get; set; }
        IConnectionManager Manager { get; set; }
        IConnectionProvider<TcpClient> Provider { get; set; }
        IClientDispatcher<TcpClient> Dispatcher { get; set; }
        IClientHandler<TcpClient> Handler { get; set; }

        void Start(string hostname, int port);
        void Stop();
    }
}

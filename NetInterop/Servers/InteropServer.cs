using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Connections;
using NetInterop.Transport.Sockets.Server;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NetInterop.Servers
{
    public class InteropServer : IServer
    {
        private TcpListener listener;

        public IConnection Connection { get; set; }
        public IConnectionManager Manager { get; set; }
        public IConnectionProvider<TcpClient> Provider { get; set; }
        public IClientDispatcher<TcpClient> Dispatcher { get; set; }
        public IClientHandler<TcpClient> Handler { get; set; } = new InteropClientHandler();

        public void Start(string hostname, int port)
        {
            listener = new TcpListener(IPAddress.Parse(hostname), port);

            Connection = new DefaultTcpListenerConnection(listener);

            Dispatcher = new DefaultClientDispatcher<TcpClient>(Handler);

            Provider = new DefaultClientProvider<TcpClient>(client => new DefaultTcpServerClientConnection(client));

            Manager = new DefaultTcpListenerConnectionManager(listener, Dispatcher, Provider);

            Manager.StartConnecting();
        }

        public void Stop()
        {
            Manager.StopConnecting();

            listener.Stop();

            Manager.DisconnectAll();

            Manager = null;

            Provider = null;

            Dispatcher = null;

            Connection.Disconnect();

            Connection = null;

            listener = null;
        }
    }
}

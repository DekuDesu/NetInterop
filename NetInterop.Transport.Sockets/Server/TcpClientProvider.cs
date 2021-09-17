using NetInterop.Transport.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Providers
{
    public class TcpClientProvider : IConnectionProviderOld<TcpClient>
    {
        private TcpListener server;
        private readonly IPAddress address;

        public TcpClientProvider(IPAddress address, int port)
        {
            this.address = address;
            SetPort(port);
        }

        public void SetPort(int port)
        {
            if (server is not null)
            {
                Stop();
            }
            server = new TcpListener(address, port);
        }

        public TcpClient AcceptClient() => server.AcceptTcpClient();

        public void Start() => server.Start();

        public void Stop() => server.Stop();
        public bool Pending() => server.Pending();
    }
}

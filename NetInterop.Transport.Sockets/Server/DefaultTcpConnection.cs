using NetInterop.Transport.Core.Abstractions.Connections;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Sockets.Server
{
    public class DefaultTcpConnection : IConnection
    {
        private readonly TcpClient client;
        private readonly IPAddress address;
        private readonly int port;

        public DefaultTcpConnection(TcpClient client, IPAddress address, int port)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
            this.address = address ?? throw new ArgumentNullException(nameof(address));
            this.port = port;
        }

        public bool IsConnected => client.Connected && (client.Client?.Connected ?? false);

        public event Action<IConnection> Connected;
        public event Action<IConnection> Disconnected;

        [DebuggerHidden]
        public void Connect()
        {
            client.Connect(address, port);
            Connected?.Invoke(this);
        }

        [DebuggerHidden]
        public void Disconnect()
        {
            client.Close();
            client.Dispose();
            Disconnected?.Invoke(this);
        }
    }
}

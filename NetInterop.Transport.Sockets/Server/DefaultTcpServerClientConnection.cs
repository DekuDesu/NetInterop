using NetInterop.Transport.Core.Abstractions.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Sockets.Server
{
    /// <summary>
    /// Represents a tcp client that was connected by a server
    /// </summary>
    public class DefaultTcpServerClientConnection : IConnection
    {
        private readonly TcpClient client;

        public DefaultTcpServerClientConnection(TcpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public bool IsConnected => client.Connected;

        public event Action<IConnection> Connected;
        public event Action<IConnection> Disconnected;

        public void Connect()
        {
            Connected?.Invoke(this);
        }

        public void Disconnect()
        {
            client.Close();
            client.Dispose();
        }
    }
}

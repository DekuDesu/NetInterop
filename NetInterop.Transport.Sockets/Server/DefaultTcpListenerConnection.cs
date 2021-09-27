using NetInterop.Transport.Core.Abstractions.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Sockets.Server
{
    public class DefaultTcpListenerConnection : IConnection
    {
        private readonly TcpListener listener;

        public DefaultTcpListenerConnection(TcpListener listener)
        {
            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));
        }

        public bool IsConnected { get; private set; }

        public event Action<IConnection> Connected;
        public event Action<IConnection> Disconnected;

        public void Connect()
        {
            listener.Start();
            IsConnected = true;
            Connected?.Invoke(this);
        }

        public void Disconnect()
        {
            listener.Stop();
            IsConnected = false;
            Disconnected?.Invoke(this);
        }
    }
}

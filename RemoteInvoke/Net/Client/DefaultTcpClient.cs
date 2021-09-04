using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace RemoteInvoke.Net.Client
{
    public class DefaultTcpClient : IClient<NetworkStream>
    {
        private TcpClient backingClient;
        private readonly IPAddress address;
        private readonly int port;

        public DefaultTcpClient(IPAddress address, int port)
        {
            if (address is null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            if (port <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }

            this.address = address;
            this.port = port;
            backingClient = new();
        }

        public NetworkStream? Client { get; private set; }

        public bool Connected { get; private set; }

        public bool Available => Client != null;

        public bool TryConnect([NotNullWhen(true)] out NetworkStream stream)
        {
            backingClient ??= new();

            stream = default!;

            if (Connected)
            {
                Disconnect();
            }

            try
            {
                backingClient.Connect(address, port);
                Connected = true;
            }
            // only catch expected exceptions
            catch (SocketException) { return false; }
            catch (ObjectDisposedException) { return false; }
            catch (ArgumentOutOfRangeException) { return false; }
            catch (ArgumentNullException) { return false; }

            try
            {
                Client = backingClient.GetStream();
            }
            // only catch expected exceptions
            catch (ObjectDisposedException) { return false; }
            catch (InvalidOperationException) { return false; }

            stream = Client;

            return Available;
        }

        public void Disconnect(int timeout)
        {
            Connected = false;

            Client?.Close(timeout);
            Client = null;

            backingClient?.Close();
        }

        public void Disconnect()
        {
            Disconnect(0);
        }

        public void Dispose()
        {
            Disconnect();

            backingClient?.Dispose();
            Client?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}

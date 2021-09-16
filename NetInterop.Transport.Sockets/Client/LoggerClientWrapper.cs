using NetInterop.Transport.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable
namespace NetInterop.Transport.Clients
{
    public class LoggerClientWrapper<T> : IClient<T> where T : IDisposable
    {
        private IClient<T> client;
        public LoggerClientWrapper(IClient<T> client)
        {
            this.client = client;
        }

        public T? Client => client.Client;
        public bool Connected => client.Connected;
        public bool Available => client.Available;

        public void Disconnect()
        {
            Console.WriteLine("Client: Disconnecting");

            client.Disconnect();

            Console.WriteLine("Client: Disconnected");
        }

        public void Disconnect(int timeout)
        {
            Console.WriteLine($"Client: Disconnecting with timeout {timeout}");

            client.Disconnect(timeout);

            Console.WriteLine("Client: Disconnected");
        }

        public void Dispose()
        {
            Console.WriteLine("Client: Disposing");
            client.Dispose();
            client = null!;
            Console.WriteLine("Client: Disposed");
        }

        public bool TryConnect(out T stream)
        {
            Console.WriteLine($"Client: Connecting");

            bool result = client.TryConnect(out stream);

            Console.WriteLine(result ? "Client: Connected" : "Client: Failed to connect");
            return result;
        }
    }
}

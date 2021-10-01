using NetInterop.Abstractions;
using NetInterop.Clients;
using NetInterop.Transport.Core.Abstractions.Connections;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace NetInterop.Servers
{
    public class InteropClientHandler : IClientHandler<TcpClient>
    {
        public List<IClient> Clients { get; set; } = new List<IClient>();

        public event Action<IClient> OnHandle;

        public void Handle(TcpClient client, IConnection connection)
        {
            var newClient = Interop.CreateClient(client, connection);

            newClient.Start();

            Clients.Add(newClient);

            OnHandle?.Invoke(newClient);
        }
    }
}

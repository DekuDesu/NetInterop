using NetInterop.Transport.Core.Abstractions.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Sockets.Server
{
    public class DefaultClientHandler<TClient> : IClientHandler<TClient>
    {
        private readonly Action<TClient, IConnection> action;

        public DefaultClientHandler(Action<TClient, IConnection> action)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        public void Handle(TClient client, IConnection connection)
        {
            action.Invoke(client, connection);
        }
    }
}

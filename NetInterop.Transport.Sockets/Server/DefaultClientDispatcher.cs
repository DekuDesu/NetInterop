using NetInterop.Transport.Core.Abstractions.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Sockets.Server
{
    public class DefaultClientDispatcher<TClient> : IClientDispatcher<TClient>
    {
        private readonly IClientHandler<TClient> handler;

        public DefaultClientDispatcher(IClientHandler<TClient> handler)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Dispatch(TClient client, IConnection connection)
        {
            this.handler.Handle(client, connection);
        }
    }
}

using NetInterop.Transport.Core.Abstractions.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Sockets.Server
{
    public class DefaultClientProvider<TClient> : IConnectionProvider<TClient>
    {
        private readonly Func<TClient, IConnection> provider;

        public DefaultClientProvider(Func<TClient, IConnection> provider)
        {
            this.provider = provider ?? throw new ArgumentNullException(nameof(provider));
        }

        public IConnection CreateConnection(TClient client)
        {
            return provider(client);
        }
    }
}

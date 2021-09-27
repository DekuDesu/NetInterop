using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions.Connections
{
    public interface IConnectionProvider<TClient>
    {
        IConnection CreateConnection(TClient client);
    }
}

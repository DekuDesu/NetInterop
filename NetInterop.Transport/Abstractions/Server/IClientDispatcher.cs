using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions.Server
{
    public interface IClientDispatcher<TClient>
    {
        /// <summary>
        /// Dispatches the client to be handled
        /// </summary>
        /// <param name="client"></param>
        void Dispatch(TClient client, IConnection connection);
    }
}

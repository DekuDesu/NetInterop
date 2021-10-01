using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions.Connections
{
    /// <summary>
    /// Defines an object that consumes a <typeparamref name="TClient"/> and performs runtime logic with it
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    public interface IClientHandler<TClient>
    {
        /// <summary>
        /// Consumes and handles the client for actual runtime logic
        /// </summary>
        /// <param name="client"></param>
        void Handle(TClient client, IConnection connection);
    }
}

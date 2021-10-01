using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions.Connections
{
    /// <summary>
    /// Defines an object that controls other connections
    /// </summary>
    public interface IConnectionManager
    {
        /// <summary>
        /// The number of connections
        /// </summary>
        int Count { get; }

        /// <summary>
        /// The connections that this object manages
        /// </summary>
        IReadOnlyCollection<IConnection> Connections { get; }

        bool Connecting { get; }

        /// <summary>
        /// Invoked when a new connection is registered with the manager
        /// </summary>
        event Action<IConnection> Connected;

        void StartConnecting();

        void StopConnecting();

        /// <summary>
        /// Disconnects all connections this object manages
        /// </summary>
        void DisconnectAll();
    }
}

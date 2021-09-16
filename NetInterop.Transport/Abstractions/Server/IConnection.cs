using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Abstractions.Server
{
    /// <summary>
    /// Defines an object that controlls a connection to a resource
    /// </summary>
    public interface IConnection
    {
        /// <summary>
        /// Whether or not the connection to the resource is active
        /// </summary>
        bool IsConnected { get; }

        event Action<IConnection> Connected;
        event Action<IConnection> Disconnected;

        /// <summary>
        /// Attemps to connect the resource
        /// </summary>
        void Connect();

        /// <summary>
        /// Attempts to disconnect from the resource
        /// </summary>
        void Disconnect();
    }
}

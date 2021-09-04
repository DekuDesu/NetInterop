using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
#nullable enable
namespace RemoteInvoke.Net
{
    /// <summary>
    /// Controls opening, keeping track of connections, and closing them
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IServer<T>
    {
        /// <summary>
        /// The connected clients to this server
        /// </summary>
        IReadOnlyCollection<T> ConnectedClients { get; }

        /// <summary>
        /// Whether or not the server is currently started, use <see cref="Start{T}"/> to start the server.
        /// </summary>
        bool IsStarted { get; }

        /// <summary>
        /// The port the server is currently using
        /// </summary>
        int Port { get; }

        event Action<T>? ClientConnected;
        event Action? Started;
        event Action? Stopped;
        event Action? Cancelled;

        /// <summary>
        /// Blocking; Waits until a client is connecting, connects the client, and returns the new client
        /// </summary>
        /// <returns></returns>
        T? AcceptClient();

        /// <summary>
        /// Blocking; Connects any clients attempting to connect, when a client connects <paramref name="clientDispatcher"/> is invoked with the new client
        /// </summary>
        /// <param name="clientDispatcher"></param>
        /// <param name="token"></param>
        void BeginAcceptingClients(Action<T> clientDispatcher, CancellationToken token);

        /// <summary>
        /// Cancels accepting clients
        /// </summary>
        void Cancel();

        /// <summary>
        /// Cancels accepting clients
        /// </summary>
        /// <param name="token"></param>
        void Cancel(CancellationToken token);

        /// <summary>
        /// Disposes all connections
        /// </summary>
        void CloseConnections();

        /// <summary>
        /// Starts the server
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the server, cancels any blocking tasks for this object
        /// <code>
        /// !!! DOES NOT CLOSE ANY OPEN CONNECTIONS !!!
        /// </code>
        /// Use <see cref="CloseConnections"/> to dispose of any open connections.
        /// </summary>
        void Stop();
    }
}
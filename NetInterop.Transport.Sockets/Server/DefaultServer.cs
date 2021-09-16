using NetInterop.Transport.Core.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace NetInterop.Transport.Servers
{
    public class DefaultServer<T> : IServer<T> where T : IDisposable
    {
        private readonly IClientProvider<T> server;
        private readonly IList<T> _connectedClients = new List<T>();
        /// <summary>
        /// the time inbetween loop body iterations in this object
        /// </summary>
        private const int c_pollingRate = 10;
        /// <summary>
        /// forces all loops to exit gracefully
        /// </summary>
        private volatile bool forceCancelSentinel;
        /// <summary>
        /// Always signaled unless a breaking change to this objects state is happening
        /// </summary>
        private readonly EventWaitHandle breakingChangeLock = new(true, EventResetMode.ManualReset);

        public int Port { get; private set; }

        public bool IsStarted { get; private set; }

        public bool IsAcceptingClients { get; private set; }

        public IReadOnlyCollection<T> ConnectedClients => (IReadOnlyCollection<T>)_connectedClients;

        public event Action? Started;
        public event Action? Stopped;
        public event Action<T>? ClientConnected;
        public event Action? Cancelled;

        public DefaultServer(int port, IClientProvider<T> clientProvider)
        {
            Port = port;
            this.server = clientProvider;
            clientProvider.SetPort(port);
        }

        public void SetPort(int port)
        {
            SetPort(port, CancellationToken.None);
        }

        public void SetPort(int port, CancellationToken token)
        {
            InvokeBreaking(() =>
            {
                bool wasStarted = IsStarted;

                // stop the server
                Stop();

                server.SetPort(port);

                this.Port = port;

                if (wasStarted)
                {
                    Start();
                }
            }, token);
        }

        public T? AcceptClient()
        {
            if (IsStarted is false)
            {
                throw new InvalidOperationException("Attempted to accept clients before server has started.");
            }

            if (server.Pending())
            {
                T client = server.AcceptClient();

                if (client is not null)
                {
                    _connectedClients.Add(client);

                    ClientConnected?.Invoke(client);

                    return client;
                }
            }

            return default;
        }

        public void BeginAcceptingClients(Action<T> clientDispatcher, CancellationToken token)
        {


            InvokeOrBlock(() =>
            {
                IsAcceptingClients = true;

                while (token.IsCancellationRequested is false && forceCancelSentinel is false)
                {
                    T? client = AcceptClient();

                    if (client is not null)
                    {
                        clientDispatcher(client);
                    }

                    Thread.Sleep(c_pollingRate);
                }

                IsAcceptingClients = false;
            });
        }

        public void Start()
        {
            server.Start();

            IsStarted = true;

            Started?.Invoke();
        }

        public void Stop()
        {
            Cancel();

            server.Stop();

            IsStarted = false;

            Stopped?.Invoke();
        }

        public void CloseConnections()
        {
            foreach (var item in _connectedClients)
            {
                item?.Dispose();
            }

            _connectedClients.Clear();
        }

        public void Cancel()
        {
            Cancel(CancellationToken.None);
        }

        public void Cancel(CancellationToken token)
        {
            // tell all loops to exit
            forceCancelSentinel = true;

            while (IsAcceptingClients)
            {
                // we throw a OCE instead of graceful exit becuase we can't garuntee the state of the object when they cancel the cancellation
                token.ThrowIfCancellationRequested();

                Thread.Sleep(c_pollingRate);
            }

            // allow more loops to begin
            forceCancelSentinel = false;

            Cancelled?.Invoke();
        }

        /// <summary>
        /// Forces any loops to exit, waits for them, and them performs the expression
        /// </summary>
        /// <param name="Expression"></param>
        private void InvokeBreaking(Action Expression, CancellationToken token)
        {
            // wait for any breaking changes that are already happening
            breakingChangeLock.WaitOne();

            // force all working loops to exit gracefully
            Cancel(token);

            // force any new work to block while we perform work
            breakingChangeLock.Reset();
            try
            {
                Expression();
            }
            finally
            {
                // release lock even if we encounter an error
                breakingChangeLock.Set();
            }
        }

        private void InvokeOrBlock(Action Expression)
        {
            // wait for any breaking changes to finish before we begin
            breakingChangeLock.WaitOne();

            Expression();
        }
    }
}
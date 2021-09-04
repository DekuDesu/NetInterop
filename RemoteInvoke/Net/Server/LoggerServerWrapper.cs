using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#nullable enable
namespace RemoteInvoke.Net.Server
{
    public class LoggerServerWrapper<T> : IServer<T> where T : IDisposable
    {
        private readonly IServer<T> backingServer;

        public LoggerServerWrapper(IServer<T> backingServer)
        {
            this.backingServer = backingServer;

            // redirect the events
            backingServer.ClientConnected += (client) => ClientConnected?.Invoke(client);
            backingServer.Started += () => Started?.Invoke();
            backingServer.Stopped += () => Stopped?.Invoke();
            backingServer.Cancelled += () => Cancelled?.Invoke();
        }

        public IReadOnlyCollection<T> ConnectedClients => backingServer.ConnectedClients;
        public bool IsStarted => backingServer.IsStarted;
        public int Port => backingServer.Port;

        public event Action<T>? ClientConnected;
        public event Action? Started;
        public event Action? Stopped;
        public event Action? Cancelled;

        public T? AcceptClient()
        {
            T? result = backingServer.AcceptClient();

            if (result is not null)
            {
                Console.WriteLine("Server: connected pending client");
            }

            return result;
        }

        public void BeginAcceptingClients(Action<T> clientDispatcher, CancellationToken token)
        {
            Console.WriteLine("Server: Listening for new clients");

            backingServer.BeginAcceptingClients((client) =>
            {
                Console.WriteLine("Server: connected pending client");
                clientDispatcher(client);
            }, token);

            Console.WriteLine("Server: Stopped listening for new clients");
        }

        public void Cancel()
        {
            Cancel(CancellationToken.None);
        }

        public void Cancel(CancellationToken token)
        {
            Console.WriteLine("Server: Cancelling long running operations");

            backingServer.Cancel(token);

            Console.WriteLine("Stopped: All long running operations cancelled");
        }

        public void CloseConnections()
        {
            Console.WriteLine("Server: Closing all connections");

            backingServer.CloseConnections();

            Console.WriteLine("Stopped: All connections closed");
        }

        public void Start()
        {
            Console.WriteLine("Server: Starting server");

            backingServer.Start();

            Console.WriteLine("Stopped: Server started");
        }

        public void Stop()
        {
            Console.WriteLine("Server: Stopping server");

            backingServer.Stop();

            Console.WriteLine("Stopped: Server stopped");
        }
    }
}

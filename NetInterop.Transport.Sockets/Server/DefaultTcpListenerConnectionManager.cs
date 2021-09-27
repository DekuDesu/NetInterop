using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Connections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace NetInterop.Transport.Sockets.Server
{
    public class DefaultTcpListenerConnectionManager : IConnectionManager
    {
        private readonly List<IConnection> clients = new List<IConnection>();
        private readonly TcpListener listener;
        private Timer acceptTimer;
        private readonly IClientDispatcher<TcpClient> dispatcher;
        private readonly IConnectionProvider<TcpClient> connectionProvider;
        private readonly object listSynchronizationObject = new object();

        public DefaultTcpListenerConnectionManager(TcpListener listener, IClientDispatcher<TcpClient> dispatcher, IConnectionProvider<TcpClient> connectionProvider)
        {
            if (connectionProvider is null)
            {
                throw new ArgumentNullException(nameof(connectionProvider));
            }

            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.connectionProvider = connectionProvider;
        }

        /// <summary>
        /// The time inbetween connection attempts in ms (milliseconds).
        /// </summary>
        public double ConnectionAttemptInterval { get; set; } = 100;

        public int Count => clients.Count;

        public bool Connecting { get; private set; }

        public IReadOnlyCollection<IConnection> Connections => clients;

        public event Action<IConnection> Connected;

        public void DisconnectAll()
        {
            lock (listSynchronizationObject)
            {
                foreach (var item in clients)
                {
                    item.Disconnect();
                }
            }
            listener.Stop();
            acceptTimer?.Stop();
            acceptTimer?.Dispose();
            Connecting = false;
        }

        public void StartConnecting()
        {
            StartTimer();
        }

        public void StopConnecting()
        {
            StopTimer();
            Connecting = false;
        }

        private void StartTimer()
        {
            if (acceptTimer != null)
            {
                StopTimer();
            }

            acceptTimer = new System.Timers.Timer() { Interval = ConnectionAttemptInterval, AutoReset = true, Enabled = true };
            acceptTimer.Elapsed += AcceptEvent;

            listener.Start();

            Connecting = true;

            acceptTimer.Start();
        }

        private void StopTimer()
        {
            acceptTimer?.Stop();
            acceptTimer?.Dispose();
            acceptTimer = null;
            Connecting = false;
        }

        /// <summary>
        /// Invoked when the timer elapses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AcceptEvent(object sender, ElapsedEventArgs e)
        {
            if (clients.Count != 0)
            {
                CheckClientConnections();
            }

            // make sure we aren't accepting clients becuase of a timer race condition
            if (Connecting == false)
            {
                return;
            }

            bool pending;

            try
            {
                pending = listener.Pending();
            }
            catch (ObjectDisposedException)
            {
                StopConnecting();
                return;
            }

            if (pending)
            {
                TcpClient client = listener?.AcceptTcpClient();

                if (client is null)
                {
                    return;
                }

                IConnection connection = connectionProvider.CreateConnection(client);

                connection.Connect();

                lock (listSynchronizationObject)
                {
                    clients.Add(connection);
                }

                dispatcher.Dispatch(client, connection);

                Connected?.Invoke(connection);
            }
        }

        private void CheckClientConnections()
        {
            // this is unlikely to have a significant performance impact, each IConnection copied is only the size of a long
            IConnection[] safeClientsCopy;

            lock (listSynchronizationObject)
            {
                safeClientsCopy = clients.ToArray();
            }

            foreach (var item in safeClientsCopy)
            {
                if (item.IsConnected is false)
                {
                    lock (listSynchronizationObject)
                    {
                        item.Disconnect();
                        clients.Remove(item);
                    }
                }
            }
        }
    }
}

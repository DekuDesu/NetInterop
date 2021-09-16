using NetInterop.Transport.Core.Abstractions.Server;
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
        private readonly List<IConnection> clients = new();
        private readonly TcpListener listener;
        private Timer acceptTimer;
        private IClientDispatcher<TcpClient> dispatcher;


        public DefaultTcpListenerConnectionManager(TcpListener listener, IClientDispatcher<TcpClient> dispatcher)
        {
            this.listener = listener ?? throw new ArgumentNullException(nameof(listener));
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
        }

        /// <summary>
        /// The time inbetween connection attempts in ms (milliseconds).
        /// </summary>
        public double ConnectionAttemptInterval { get; init; } = 100;

        public int Count => clients.Count;

        public bool Connecting { get; private set; }

        public IReadOnlyCollection<IConnection> Connections => clients;

        public event Action<IConnection> Connected;

        public void DisconnectAll()
        {
            foreach (var item in clients)
            {
                item.Disconnect();
            }
            listener.Stop();
            acceptTimer.Stop();
            acceptTimer.Dispose();
            Connecting = false;
        }

        public void StartConnecting()
        {
            StartTimer();
        }

        public void StopConnecting()
        {
            StopTimer();
        }

        private void StartTimer()
        {
            if (acceptTimer is not null)
            {
                StopTimer();
            }

            acceptTimer = new() { Interval = ConnectionAttemptInterval, AutoReset = true, Enabled = true };
            acceptTimer.Elapsed += AcceptEvent;

            listener.Start();

            Connecting = true;

            acceptTimer.Start();
        }

        private void StopTimer()
        {
            acceptTimer.Stop();
            acceptTimer.Dispose();
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
            // make sure we aren't accepting clients becuase of a timer race condition
            if (Connecting == false)
            {
                StopTimer();
                return;
            }

            // check to see if there are any clients waiting to connect
            if (listener.Pending())
            {
                TcpClient client = listener.AcceptTcpClient();

                IConnection connection = new DefaultTcpServerClientConnection(client);

                connection.Disconnected += (connection) => clients.Remove(connection);

                clients.Add(connection);

                dispatcher.Dispatch(client, connection);

                Connected?.Invoke(connection);
            }
        }
    }
}

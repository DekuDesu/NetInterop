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
    /// <summary>
    /// Represents a tcp client that was connected by a server
    /// </summary>
    public class DefaultTcpServerClientConnection : IConnection
    {
        private readonly TcpClient client;
        private readonly int interval = 100;
        private Timer connectedTimer;

        public DefaultTcpServerClientConnection(TcpClient client)
        {
            this.client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public bool IsConnected => client.Connected && client.Client.Connected;

        public event Action<IConnection> Connected;
        public event Action<IConnection> Disconnected;

        public void Connect()
        {
            StartTimer();
            Connected?.Invoke(this);
        }

        public void Disconnect()
        {
            StopTimer();
            client.Close();
            client.Dispose();
            Disconnected?.Invoke(this);
        }

        private void StartTimer()
        {
            if (connectedTimer is not null)
            {
                StopTimer();
            }

            connectedTimer = new Timer() { AutoReset = true, Enabled = true, Interval = interval };
            connectedTimer.Elapsed += TimerEvent;

            connectedTimer.Start();
        }

        private void StopTimer()
        {
            connectedTimer.Stop();
            connectedTimer.Dispose();
        }

        /// <summary>
        /// Checks the status of client, if its disconnected then it closes the connection and disposed the timer
        /// </summary>
        /// <param name="caller"></param>
        /// <param name="args"></param>
        private void TimerEvent(object caller, ElapsedEventArgs args)
        {
            if (client.Connected is false)
            {
                Disconnect();
                return;
            }

            try
            {
                client.GetStream().Write(Array.Empty<byte>());
            }
            catch (Exception)
            {
                Disconnect();
            }
        }
    }
}

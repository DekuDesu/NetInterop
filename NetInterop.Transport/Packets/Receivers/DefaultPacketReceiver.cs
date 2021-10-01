using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Connections;
using NetInterop.Transport.Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace NetInterop.Transport.Core.Packets
{
    public class DefaultPacketReceiver : IPacketReceiver
    {
        private readonly IPacketController controller;
        private readonly IConnection connection;
        private readonly IPacketHandler handler;
        private System.Timers.Timer packetAvailableTimer;
        private readonly double timerInterval = 1000 / 60;
        public DefaultPacketReceiver(IPacketHandler handler, IPacketController controller, IConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.connection = connection;
        }

        public void BeginReceiving()
        {
            StartTimer();

            // immediate try once to consume a packet while the timer starts
            TryConsumePacket();
        }

        public void StopReceiving()
        {
            StopTimer();
        }

        private bool TryConsumePacket()
        {
            if (controller.PendingPackets)
            {
                if (controller.TryReadPacket(out IPacket packet))
                {
                    handler.Handle(packet);
                    return true;
                }
            }
            return false;
        }

        private void StartTimer()
        {
            if (packetAvailableTimer != null)
            {
                StopTimer();
            }
            packetAvailableTimer = new System.Timers.Timer() { Interval = timerInterval, AutoReset = true, Enabled = true };
            packetAvailableTimer.Elapsed += TimerEvent;
            packetAvailableTimer.Start();
        }

        private void StopTimer()
        {
            packetAvailableTimer?.Stop();
            packetAvailableTimer?.Dispose();
            packetAvailableTimer = null;
        }

        private void TimerEvent(object caller, object args)
        {
            if (connection.IsConnected is false)
            {
                StopReceiving();
                return;
            }
            TryConsumePacket();
        }

        public bool Receive()
        {
            return TryConsumePacket();
        }

        public async Task ReceiveAsync(CancellationToken token)
        {
            while (token.IsCancellationRequested is false)
            {
                TryConsumePacket();
                await Task.Delay(1);
            }
        }

        public void ReceiveMany(CancellationToken token)
        {
            while (token.IsCancellationRequested is false)
            {
                TryConsumePacket();
                Thread.Sleep(1);
            }
        }

        public IEnumerator<bool> ReceiveMany()
        {
            while (true)
            {
                yield return TryConsumePacket();
            }
        }
    }
}

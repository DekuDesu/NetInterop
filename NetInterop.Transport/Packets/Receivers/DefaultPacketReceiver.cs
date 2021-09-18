using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Server;
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
    public class DefaultPacketReceiver<TPacketType> : IPacketReceiver<TPacketType> where TPacketType : Enum, IConvertible
    {
        private readonly IPacketController<TPacketType> controller;
        private readonly IConnection connection;
        private readonly IPacketDispatcher<TPacketType> dispatcher;
        private System.Timers.Timer packetAvailableTimer;
        private double timerInterval = 1000 / 60;
        public DefaultPacketReceiver(IPacketDispatcher<TPacketType> dispatcher, IPacketController<TPacketType> controller, IConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
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

        private void TryConsumePacket()
        {
            if (controller.PendingPackets)
            {
                if (controller.TryReadPacket(out IPacket<TPacketType> packet))
                {
                    dispatcher.Dispatch(packet);
                }
            }
        }

        private void StartTimer()
        {
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
    }
}

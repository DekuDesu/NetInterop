using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetInterop.Runtime.Jobs
{
    /// <summary>
    /// Performs a long-running job that consumes and keeps open a single thread, that thread constantly handles receiving and decoding network packets
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="controller"></param>
    /// <param name="token"></param>
    public class PacketReceiverJob : IWork
    {
        private readonly IPacketReceiver receiver;
        private readonly IPacketController controller;

        /// <summary>
        /// Performs a long-running job that consumes and keeps open a single thread, that thread constantly handles receiving and decoding network packets
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="controller"></param>
        /// <param name="token"></param>
        public PacketReceiverJob(IPacketReceiver receiver, IPacketController controller)
        {
            this.receiver = receiver;
            this.controller = controller;
        }

        public void PerformWork(CancellationToken token)
        {
            while (token.IsCancellationRequested is false)
            {
                while (controller.PendingPackets && token.IsCancellationRequested is false)
                {
                    receiver.Receive();
                }

                Thread.Sleep(1000 / 60);
            }
        }
    }
}

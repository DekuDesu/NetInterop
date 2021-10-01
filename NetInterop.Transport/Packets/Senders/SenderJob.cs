using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace NetInterop.Transport.Core.Packets.Senders
{
    public class SenderJob : IWork
    {
        private readonly IPacketSerializable packet;
        private readonly IPacketSender sender;

        public SenderJob(IPacketSerializable packet, IPacketSender sender)
        {
            this.packet = packet ?? throw new ArgumentNullException(nameof(packet));
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        public void PerformWork(CancellationToken token)
        {
            sender.Send(packet);
        }
    }
}

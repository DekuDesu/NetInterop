using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Transport.Core.Packets.Senders
{
    public class PacketWorkPoolSender : IPacketSender
    {
        private readonly IPacketSender sender;
        private readonly IWorkPool pool;

        public PacketWorkPoolSender(IPacketSender sender, IWorkPool pool)
        {
            this.sender = sender ?? throw new ArgumentNullException(nameof(sender));
            this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
        }

        public void Send(IPacketSerializable value)
        {
            pool.AddWork(new SenderJob(value, sender));
        }
    }
}

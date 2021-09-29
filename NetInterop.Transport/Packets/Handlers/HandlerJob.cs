using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Transport.Core.Packets.Handlers
{
    public class HandlerJob : IWork
    {
        private readonly IPacketHandler handler;
        private readonly IPacket packet;

        public HandlerJob(IPacketHandler handler, IPacket packet)
        {
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
            this.packet = packet ?? throw new ArgumentNullException(nameof(packet));
        }

        public void PerformWork()
        {
            handler.Handle(packet);
        }
    }
}

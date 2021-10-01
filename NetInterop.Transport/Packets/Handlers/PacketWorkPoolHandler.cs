using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Abstractions.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Transport.Core.Packets.Handlers
{
    public class PacketWorkPoolHandler : IPacketHandler
    {
        private readonly IWorkPool pool;
        private readonly IPacketHandler handler;

        public PacketWorkPoolHandler(IWorkPool pool, IPacketHandler handler)
        {
            this.pool = pool ?? throw new ArgumentNullException(nameof(pool));
            this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        }

        public void Handle(IPacket packet)
        {
            pool.AddWork(new HandlerJob(handler, packet));
        }
    }
}

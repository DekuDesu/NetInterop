using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NetInterop.Transport.Core.Factories
{
    public static class Packet
    {
        public static IPacket Empty => new DefaultPacket(0);

        public static IPacket Create(int estimatedSize)
        {
            return new DefaultPacket(estimatedSize);
        }
    }
}

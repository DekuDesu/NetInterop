using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using System;

namespace NetInterop.Transport.Core.Factories
{
    public static class Packet
    {
        public static IPacket<TContext> Create<TContext>(TContext packetType)
            where TContext : Enum, IConvertible
        {
            return new DefaultPacket<TContext>(packetType, 0);
        }

        public static IPacket<TContext> Create<TContext>(TContext packetType, byte[] data)
            where TContext : Enum, IConvertible
        {
            return new DefaultPacket<TContext>(packetType, data, DefaultPacket<TContext>.DefaultHeaderSize);
        }
        public static IPacket<TContext> Empty<TContext>(TContext packetType)
            where TContext : Enum, IConvertible
        {
            return new DefaultPacket<TContext>(packetType, Array.Empty<byte>(), sizeof(uint));
        }
        public static IPacket<TContext> Create<TContext>(TContext packetType, int estimatedSize)
            where TContext : Enum, IConvertible
        {
            return new DefaultPacket<TContext>(packetType, estimatedSize);
        }
    }
}

using System;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Delegates;

namespace NetInterop.Transport.Core.Abstractions
{
    public interface IPacketHeader<TPacket> where TPacket : Enum, IConvertible
    {
        void CreateHeader(ref Packet<TPacket> packet);
        int GetPacketSize(Span<byte> headerBytes);
        TPacket GetHeaderType(Span<byte> headerBytes);
    }
}
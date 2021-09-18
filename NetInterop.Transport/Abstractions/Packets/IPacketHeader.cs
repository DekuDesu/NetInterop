using System;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPacketHeader<TPacket> where TPacket : Enum, IConvertible
    {
        void CreateHeader(IPacket<TPacket> packet);
        int GetPacketSize(ref byte headerPtr);
        TPacket GetHeaderType(ref byte headerPtr);
    }
}
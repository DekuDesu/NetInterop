using System;
using NetInterop.Transport.Core.Packets;
using NetInterop.Transport.Core.Delegates;

namespace NetInterop.Transport.Core.Abstractions
{
    public interface IPacketHeader<TPacket> where TPacket : Enum, IConvertible
    {
        void CreateHeader(IPacket<TPacket> packet);
        int GetPacketSize(ref byte headerPtr);
        TPacket GetHeaderType(ref byte headerPtr);
    }
}
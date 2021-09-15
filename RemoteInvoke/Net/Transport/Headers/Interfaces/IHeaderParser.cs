using System;

namespace RemoteInvoke.Net.Transport
{
    public interface IHeaderParser
    {
        uint CreateHeader(ushort packetSize, byte packetType);
        uint CreateLargeHeader(int packetSize, byte packetType);
        int GetPacketType(uint header);
        int GetPacketSize(uint header);
        void CreateHeader<TPacketType>(ref Packet<TPacketType> packet) where TPacketType : Enum, IConvertible;
        int GetPacketSize(Span<byte> headerBytes);
        TPacketType GetHeaderType<TPacketType>(Span<byte> headerBytes) where TPacketType : Enum, IConvertible;
    }
}
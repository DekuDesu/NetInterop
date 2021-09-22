using System;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPacket<TContext> : IPacket where TContext : Enum
    {
        TContext PacketType { get; set; }
    }

    public interface IPacket
    {
        int ActualSize { get; }
        int Length { get; }

        byte[] GetData();

        void Append(byte[] newData);
        ref byte GetBuffer(int length);
        ref byte GetHeader();
        ref byte Remove(int length);
        void SetHeader(byte[] header);
    }
}
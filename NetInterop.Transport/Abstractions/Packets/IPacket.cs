using System;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPacket<TContext> where TContext : Enum
    {
        int ActualSize { get; }
        int Length { get; }
        TContext PacketType { get; set; }

        byte[] GetData();

        void Append(byte[] newData);
        ref byte GetBuffer(int length);
        ref byte GetHeader();
        ref byte Remove(int length);
        void SetHeader(byte[] header);
    }
}
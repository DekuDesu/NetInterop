using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RemoteInvoke.Net.Transport.Packets.Extensions;

namespace RemoteInvoke.Net.Transport
{
    /// <summary>
    /// Represents a wrapper around a <see cref="Span{TPayload}"/> with an indentifying number for the context the <see cref="Data"/> represents
    /// </summary>
    public ref struct Packet<TContext>
        where TContext : Enum, IConvertible
    {
        /// <summary>
        /// The type of packet this represents, this is enum whos value must be between 0 and 255(byte)
        /// </summary>
        public TContext PacketType { get; set; }

        /// <summary>
        /// The data of the packet
        /// </summary>
        public Span<byte> Data;

        public int Length => Data.Length;

        public Packet(TContext packetType, Span<byte> data)
        {
            PacketType = packetType;
            Data = data;
        }
    }

    public static class Packet
    {
        public static Packet<TContext> Create<TContext>(TContext packetType)
            where TContext : Enum, IConvertible
        {
            return new Packet<TContext>() { PacketType = packetType };
        }
    }
}

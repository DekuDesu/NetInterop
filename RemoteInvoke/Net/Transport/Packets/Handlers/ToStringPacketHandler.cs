using RemoteInvoke.Net.Transport.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static RemoteInvoke.Net.Transport.Packets.PacketControllerExtensions;

namespace RemoteInvoke.Net.Transport.Packets
{
    public class ToStringPacketHandler<TPacketType> : IPacketHandler<TPacketType> where TPacketType : Enum, IConvertible
    {
        private readonly PacketRefFunc<string, TPacketType> converter;

        public ToStringPacketHandler(TPacketType packetType, PacketRefFunc<string, TPacketType> converter)
        {
            this.converter = converter ?? throw new ArgumentNullException(nameof(converter));
            PacketType = packetType;
        }

        public TPacketType PacketType { get; set; }

        public void Handle(ref Packet<TPacketType> packet)
        {
            Console.WriteLine(converter(ref packet));
        }
    }
}

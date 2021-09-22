using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Pointers
{
    public class DefaultPointerPacketHandler<TPacket> : IPacketHandler<TPacket> where TPacket : Enum, IConvertible
    {
        private INetworkTypeHandler typeHandler;
        private IPointerProvider pointerProvider;

        public TPacket PacketType { get; } = default;

        public void Handle(IPacket<TPacket> packet)
        {
            PointerOperations operation = (PointerOperations)packet.GetByte();

            INetPtr ptr = default;

            switch (operation)
            {
                case PointerOperations.Alloc:
                    ptr = pointerProvider.Create(packet.GetUShort(), 0);
                    break;
                case PointerOperations.Set:
                case PointerOperations.Get:
                    ptr = pointerProvider.Create(packet.GetUShort(), packet.GetUShort());
                    break;
                case PointerOperations.none:
                default:
                    return;
            }

            INetworkType type = typeHandler.GetAmbiguousNetworkType(ptr);

            switch (operation)
            {
                case PointerOperations.Alloc:
                    INetPtr newPtr = type.AllocPtr();

                    // send ptr back?

                    throw new NotImplementedException();
                    break;
                case PointerOperations.Set:
                    // get a deserializer for the ptr type
                    IPacketDeserializer<TPacket> deserializer = null;

                    type.SetPtr(ptr, deserializer.AmbiguousDeserialize(packet));

                    throw new NotImplementedException();
                    break;
                case PointerOperations.Get:

                    object value = type.GetPtr(ptr);

                    //IPacketSerializer < TPacket >

                    throw new NotImplementedException();
                    break;
                case PointerOperations.Invoke:
                    break;
            }


        }
    }
}

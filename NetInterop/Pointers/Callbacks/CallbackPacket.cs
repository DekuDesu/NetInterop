using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    /// <summary>
    /// Registers the given callback with the delegate handler and obtains and appends the callback id to the front of the packet
    /// </summary>
    /// <typeparam name="TPacket"></typeparam>
    public class CallbackPacket : IPacketSerializable
    {
        private readonly Action<bool, IPacket> callback;
        private readonly IDelegateHandler<bool, IPacket> packetCallbackHandler;
        private readonly IPacketSerializable packet;

        /// <summary>
        /// Registers the given callback with the delegate handler and obtains and appends the callback id to the front of the packet
        /// </summary>
        /// <typeparam name="TPacket"></typeparam>
        public CallbackPacket(Action<bool, IPacket> callback, IPacketSerializable packet, IDelegateHandler<bool, IPacket> packetCallbackHandler)
        {
            this.packet = packet ?? throw new ArgumentNullException(nameof(packet));
            this.packetCallbackHandler = packetCallbackHandler ?? throw new ArgumentNullException(nameof(packetCallbackHandler));
            this.callback = callback;
        }

        public int EstimatePacketSize() => sizeof(ushort) + packet.EstimatePacketSize();

        public void Serialize(IPacket packetBuilder)
        {
            if (callback is null)
            {
                packetBuilder.AppendUShort(0);
            }
            else
            {
                packetBuilder.AppendUShort(packetCallbackHandler.Register(callback));
            }

            packet.Serialize(packetBuilder);
        }
    }
}

using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class InvokePointerWithInstance : IPacketSerializable
    {
        private readonly INetPtr methodPtr;
        private readonly INetPtr instancePtr;

        public InvokePointerWithInstance(INetPtr methodPtr, INetPtr instancePtr)
        {
            this.methodPtr = methodPtr;
            this.instancePtr = instancePtr;
        }

        public int EstimatePacketSize() => methodPtr.EstimatePacketSize() * 2;

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendSerializable(methodPtr);
            packetBuilder.AppendSerializable(instancePtr);
        }
    }
}

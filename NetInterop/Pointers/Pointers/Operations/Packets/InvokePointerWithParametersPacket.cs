using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class InvokePointerWithParametersPacket : IPacketSerializable
    {
        private readonly object[] parameters;
        private readonly IPacketSerializer<object[]> serializer;
        private readonly INetPtr methodPtr;

        public InvokePointerWithParametersPacket(INetPtr methodPtr, IPacketSerializer<object[]> serializer, object[] parameters)
        {
            this.methodPtr = methodPtr;
            this.serializer = serializer;
            this.parameters = parameters;
        }

        public int EstimatePacketSize() => (sizeof(int) * parameters.Length) + sizeof(int); // arbitrary

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendSerializable(methodPtr);
            serializer.Serialize(parameters, packetBuilder);
        }
    }
}

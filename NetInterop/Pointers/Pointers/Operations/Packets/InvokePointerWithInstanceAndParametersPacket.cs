using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class InvokePointerWithInstanceAndParametersPacket : IPacketSerializable
    {
        private readonly INetPtr methodPtr;
        private readonly INetPtr instancePtr;
        private readonly object[] parameters;
        private readonly IPacketSerializer<object[]> serializer;

        public InvokePointerWithInstanceAndParametersPacket(INetPtr methodPtr, INetPtr instancePtr, IPacketSerializer<object[]> serializer, object[] parameters)
        {
            this.methodPtr = methodPtr;
            this.instancePtr = instancePtr;
            this.serializer = serializer;
            this.parameters = parameters;
        }

        public int EstimatePacketSize() => (methodPtr.EstimatePacketSize() * 2) + (parameters.Length * sizeof(int));

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendSerializable(methodPtr);
            packetBuilder.AppendSerializable(instancePtr);
            serializer.Serialize(parameters, packetBuilder);
        }
    }
}

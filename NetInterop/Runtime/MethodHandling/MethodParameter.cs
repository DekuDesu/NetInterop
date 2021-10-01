using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NetInterop.Runtime.MethodHandling
{
    public class MethodParameter : IPacketSerializer, IPacketDeserializer
    {
        private readonly IPacketSerializer serializer;
        private readonly IPacketDeserializer deserializer;

        public Type ParameterType { get; private set; }
        public ParameterInfo Parameter { get; }
        public bool IsVoid { get; private set; }

        public MethodParameter(ParameterInfo info, IPacketSerializer serializer, IPacketDeserializer deserializer)
        {
            Parameter = info ?? throw new ArgumentNullException(nameof(info));
            this.serializer = serializer;
            this.deserializer = deserializer;
            this.ParameterType = info.ParameterType;
            IsVoid = this.ParameterType == typeof(void);
        }

        public object AmbiguousDeserialize(IPacket packet) => deserializer?.AmbiguousDeserialize(packet);

        public void AmbiguousSerialize(object value, IPacket packetBuilder) => serializer?.AmbiguousSerialize(value, packetBuilder);
    }
}

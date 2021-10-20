using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Packets.Extensions;

namespace NetInterop.Example.Serializers
{
    public class HelloWorldSerializer :
            IActivator<HelloWorld>,
            IDeactivator<HelloWorld>,
            IPacketSerializer<HelloWorld>,
            IPacketDeserializer<HelloWorld>
    {
        private readonly IPacketSerializer<string> stringSerializer = UTF8Serializer.Instance;
        private readonly IPacketDeserializer<string> stringDeserializer = UTF8Serializer.Instance;

        public object AmbiguousDeserialize(IPacket packet) => Deserialize(packet);

        public void AmbiguousSerialize(object value, IPacket packetBuilder) => Serialize((HelloWorld)value, packetBuilder);

        public HelloWorld CreateInstance() => new();

        public HelloWorld Deserialize(IPacket packet) => new() { Message = stringDeserializer.Deserialize(packet) };

        public void DestroyInstance(ref HelloWorld instance) { instance = null; }

        public void DestroyInstance(ref object instance)
        {
            if (instance is HelloWorld isTestClass)
            {
                DestroyInstance(ref isTestClass);
            }
        }

        public int EstimatePacketSize(HelloWorld value) => stringSerializer.EstimatePacketSize(value.Message);

        public void Serialize(HelloWorld value, IPacket packetBuilder) => stringSerializer.Serialize(value.Message, packetBuilder);
    }
}

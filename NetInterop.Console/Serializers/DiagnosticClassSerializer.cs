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
    public class DiagnosticClassSerializer :
            IActivator<DiagnosticClass>,
            IDeactivator<DiagnosticClass>,
            IPacketSerializer<DiagnosticClass>,
            IPacketDeserializer<DiagnosticClass>
    {
        public static DiagnosticClassSerializer Instance { get; private set; }
        static DiagnosticClassSerializer()
        {
            Instance ??= new DiagnosticClassSerializer();
        }
        private DiagnosticClassSerializer()
        {

        }
        public object AmbiguousDeserialize(IPacket packet) => Deserialize(packet);

        public void AmbiguousSerialize(object value, IPacket packetBuilder) => Serialize((DiagnosticClass)value, packetBuilder);

        public DiagnosticClass CreateInstance()
        {
            return new DiagnosticClass();
        }

        public DiagnosticClass Deserialize(IPacket packet)
        {
            return new DiagnosticClass() { Value = packet.GetInt() };
        }

        public void DestroyInstance(ref DiagnosticClass instance)
        {
            instance?.Dispose();
        }

        public void DestroyInstance(ref object instance)
        {
            if (instance is DiagnosticClass isTestClass)
            {
                isTestClass?.Dispose();
            }
        }

        public int EstimatePacketSize(DiagnosticClass value) => sizeof(int);

        public void Serialize(DiagnosticClass value, IPacket packetBuilder)
        {
            packetBuilder.AppendInt(value.Value);
        }
    }
}

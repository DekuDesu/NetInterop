using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Serialization
{
    public abstract class BaseSerializer<T> : IPacketSerializer<T>, IPacketDeserializer<T>, IActivator<T>, IDeactivator<T>
    {
        public abstract T CreateInstance();

        public abstract void DestroyInstance(ref T instance);

        public void DestroyInstance(ref object instance)
        {
            if (instance is T isT)
            {
                DestroyInstance(ref isT);
            }
        }

        public abstract int EstimatePacketSize(T value);

        public abstract void Serialize(T value, IPacket packetBuilder);

        public object AmbiguousDeserialize(IPacket packet) => Deserialize(packet);

        public abstract T Deserialize(IPacket packet);
    }
}

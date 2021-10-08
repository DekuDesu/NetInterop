using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class DefaultSerializer<T> : IPacketSerializer<T>
    {
        private readonly Action<T, IPacket> serializeAction;
        private readonly Func<T, int> packetSizeEstimator;

        public DefaultSerializer(Action<T, IPacket> serializeAction, Func<T, int> packetSizeEstimator)
        {
            this.serializeAction = serializeAction ?? throw new ArgumentNullException(nameof(serializeAction));
            this.packetSizeEstimator = packetSizeEstimator ?? throw new ArgumentNullException(nameof(packetSizeEstimator));
        }

        public int EstimatePacketSize(T value) => packetSizeEstimator(value);

        public void Serialize(T value, IPacket packetBuilder)
        {
            serializeAction(value, packetBuilder);
        }
    }
}

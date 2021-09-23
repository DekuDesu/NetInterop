using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class NetPtr : INetPtr
    {
        public ushort InstancePtr { get; }
        public ushort PtrType { get; } = 0;

        public NetPtr() { }

        public NetPtr(ushort ptrType, ushort instancePtr)
        {
            PtrType = ptrType;
            InstancePtr = instancePtr;
        }

        public override bool Equals(object obj)
        {
            if (obj is NetPtr isNetPtr)
            {
                return isNetPtr.PtrType == this.PtrType && isNetPtr.InstancePtr == this.InstancePtr;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (PtrType << 16) + InstancePtr;
        }

        public override string ToString()
        {
            return $"{PtrType:X}{InstancePtr:X}";
        }
        public int EstimatePacketSize() => sizeof(ushort) * 2;

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendUShort(PtrType);
            packetBuilder.AppendUShort(InstancePtr);
        }

        public INetPtr Deserialize(IPacket packet)
        {
            return new NetPtr(packet.GetUShort(), packet.GetUShort());
        }
    }
    public class NetPtr<T> : NetPtr
    {
        public T Value { get; set; }

        public NetPtr(ushort ptrType, ushort instancePtr) : base(ptrType, instancePtr) { }
    }
}

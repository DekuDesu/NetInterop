using NetInterop.Attributes;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    [InteropId(0xFFFF)]
    public class NetPtr : INetPtr
    {
        public ushort PtrAddress { get; }
        public ushort PtrType { get; } = 0;

        public NetPtr() { }

        public NetPtr(ushort ptrType, ushort ptrAddress)
        {
            PtrType = ptrType;
            PtrAddress = ptrAddress;
        }

        public override bool Equals(object obj)
        {
            if (obj is NetPtr isNetPtr)
            {
                return isNetPtr.PtrType == this.PtrType && isNetPtr.PtrAddress == this.PtrAddress;
            }
            else if (obj is ushort number)
            {
                return this.PtrType == number;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (((int)PtrType) << 16) | PtrAddress;
        }

        public override string ToString()
        {
            return $"{string.Format("{0:X}", PtrType).PadLeft(2, '0')}{string.Format("{0:X}", PtrAddress).PadLeft(2, '0')}";
        }
        public int EstimatePacketSize() => sizeof(ushort) * 2;

        public void Serialize(IPacket packetBuilder)
        {
            packetBuilder.AppendUShort(PtrType);
            packetBuilder.AppendUShort(PtrAddress);
        }

        public INetPtr Deserialize(IPacket packet)
        {
            return new NetPtr(packet.GetUShort(), packet.GetUShort());
        }

        public INetPtr<T> As<T>()
        {
            if (this is INetPtr<T> netPtr)
            {
                return netPtr;
            }
            return new NetPtr<T>(PtrType, PtrAddress);
        }
    }
    public class NetPtr<T> : NetPtr, INetPtr<T>
    {
        public T Value { get; set; }

        public NetPtr(ushort ptrType, ushort ptrAddress) : base(ptrType, ptrAddress) { }
    }
}

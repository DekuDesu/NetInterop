using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop
{
    public class NetPtr : INetPtr
    {
        public ushort InstancePtr { get; }
        public ushort PtrType { get; } = 0;

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
    }
    public class NetPtr<T> : INetPtr<T>
    {
        public T Value { get; set; }

        public ushort PtrType { get; }
        public ushort InstancePtr { get; }

        public NetPtr(ushort ptrType, ushort instancePtr)
        {
            PtrType = ptrType;
            InstancePtr = instancePtr;
        }
        public override bool Equals(object obj)
        {
            if (obj is NetPtr<T> isNetPtr)
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
    }
}

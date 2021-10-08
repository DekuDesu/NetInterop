using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using NetInterop.Abstractions;
using NetInterop.Serialization;
using NetInterop.Transport.Core.Abstractions.Packets;

namespace NetInterop.Runtime
{
    public class NetTypeHandler : ITypeHandler
    {
        private readonly IPointerProvider pointerProvider;
        private readonly IDictionary<ushort, IType> types = new ConcurrentDictionary<ushort, IType>();
        private readonly IDictionary<Type, ushort> typeMap = new ConcurrentDictionary<Type, ushort>();

        public NetTypeHandler(IPointerProvider pointerProvider)
        {
            this.pointerProvider = pointerProvider;
        }

        public int Count => types.Count;

        public void RegisterPrimitiveTypes()
        {
            RegisterType((ushort)TypeCode.Boolean, BoolSerializer.Instance, BoolSerializer.Instance, BoolSerializer.Instance, BoolSerializer.Instance);
            RegisterType((ushort)TypeCode.Byte, ByteSerializer.Instance, ByteSerializer.Instance, ByteSerializer.Instance, ByteSerializer.Instance);
            RegisterType((ushort)TypeCode.SByte, SByteSerializer.Instance, SByteSerializer.Instance, SByteSerializer.Instance, SByteSerializer.Instance);
            RegisterType((ushort)TypeCode.Int16, ShortSerializer.Instance, ShortSerializer.Instance, ShortSerializer.Instance, ShortSerializer.Instance);
            RegisterType((ushort)TypeCode.UInt16, UShortSerializer.Instance, UShortSerializer.Instance, UShortSerializer.Instance, UShortSerializer.Instance);
            RegisterType((ushort)TypeCode.Int32, IntSerializer.Instance, IntSerializer.Instance, IntSerializer.Instance, IntSerializer.Instance);
            RegisterType((ushort)TypeCode.UInt32, UIntSerializer.Instance, UIntSerializer.Instance, UIntSerializer.Instance, UIntSerializer.Instance);
            RegisterType((ushort)TypeCode.Int64, LongSerializer.Instance, LongSerializer.Instance, LongSerializer.Instance, LongSerializer.Instance);
            RegisterType((ushort)TypeCode.UInt64, ULongSerializer.Instance, ULongSerializer.Instance, ULongSerializer.Instance, ULongSerializer.Instance);
            RegisterType((ushort)TypeCode.Single, FloatSerializer.Instance, FloatSerializer.Instance, FloatSerializer.Instance, FloatSerializer.Instance);
            RegisterType((ushort)TypeCode.Double, DoubleSerializer.Instance, DoubleSerializer.Instance, DoubleSerializer.Instance, DoubleSerializer.Instance);
            RegisterType((ushort)TypeCode.Decimal, DecimalSerializer.Instance, DecimalSerializer.Instance, DecimalSerializer.Instance, DecimalSerializer.Instance);
            RegisterType((ushort)TypeCode.DateTime, DateTimeSerializer.Instance, DateTimeSerializer.Instance, DateTimeSerializer.Instance, DateTimeSerializer.Instance);
        }

        public INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> deactivator, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            INetPtr<T> ptr = pointerProvider.Create<T>(interopId, 0);

            if (serializer != null && deserializer != null)
            {
                types.Add(interopId, new SerializableType<T>(new NetType<T>(ptr, activator, deactivator), serializer, deserializer));
            }
            else
            {
                types.Add(interopId, new NetType<T>(ptr, activator, deactivator));
            }

            typeMap.Add(typeof(T), interopId);

            return ptr;
        }

        public bool TryGetSerializableType(INetPtr typePtr, out ISerializableType netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out IType possiblySerializable))
            {
                if (possiblySerializable is ISerializableType isSerializable)
                {
                    netType = isSerializable;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType<T>(INetPtr typePtr, out ISerializableType<T> netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out IType possiblySerializable))
            {
                if (possiblySerializable is ISerializableType<T> isSerializable)
                {
                    netType = isSerializable;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType(Type type, out ISerializableType netType)
        {
            if (typeMap.TryGetValue(type, out ushort id))
            {
                if (types.TryGetValue(id, out IType possiblySerializable))
                {
                    if (possiblySerializable is ISerializableType isSerializable)
                    {
                        netType = isSerializable;
                        return true;
                    }
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetSerializableType<T>(out ISerializableType<T> netType)
        {
            if (typeMap.TryGetValue(typeof(T), out ushort id))
            {
                if (types.TryGetValue(id, out IType possiblyTyped))
                {
                    if (possiblyTyped is ISerializableType<T> isProperlyTyped)
                    {
                        netType = isProperlyTyped;
                        return true;
                    }
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetType(INetPtr typePtr, out IType netType) => types.TryGetValue(typePtr.PtrType, out netType);

        public bool TryGetType<T>(INetPtr typePtr, out IType<T> netType)
        {
            if (types.TryGetValue(typePtr.PtrType, out IType possiblyTyped))
            {
                if (possiblyTyped is IType<T> isProperlyTyped)
                {
                    netType = isProperlyTyped;
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetType(Type type, out IType netType)
        {
            if (typeMap.TryGetValue(type, out ushort id))
            {
                if (types.TryGetValue(id, out netType))
                {
                    return true;
                }
            }
            netType = default;
            return false;
        }

        public bool TryGetType<T>(out IType<T> netType)
        {
            if (typeMap.TryGetValue(typeof(T), out ushort id))
            {
                if (types.TryGetValue(id, out IType possiblyTyped))
                {
                    if (possiblyTyped is IType<T> isProperlyTyped)
                    {
                        netType = isProperlyTyped;
                        return true;
                    }
                }
            }
            netType = default;
            return false;
        }
    }
}

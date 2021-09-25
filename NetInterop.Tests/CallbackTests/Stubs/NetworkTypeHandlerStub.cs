using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Tests.CallbackTests.Stubs
{
    public class NetworkTypeHandlerStub : INetworkTypeHandler
    {
        public List<Type> registeredTypes = new List<Type>();
        public bool cleared = false;
        public ISerializableNetworkType network = null;
        public INetworkType networkType = null;
        public void Clear()
        {
            cleared = true;
        }

        public ushort RegisterType(Type type)
        {
            registeredTypes.Add(type);
            return (ushort)(registeredTypes.Count - 1);
        }

        public ushort RegisterType(Type type, ushort explicitId)
        {
            registeredTypes.Add(type);
            return explicitId;
        }

        public ushort RegisterType(Type type, ushort explicitId, object instantiator)
        {
            registeredTypes.Add(type);
            return explicitId;
        }

        public INetPtr<T> RegisterType<T>(ushort explicitId)
        {
            registeredTypes.Add(typeof(T));
            return new NetPtr<T>(explicitId, 0);
        }

        public INetPtr<T> RegisterType<T>(Func<T> instantiator)
        {
            registeredTypes.Add(typeof(T));
            return new NetPtr<T>((ushort)(registeredTypes.Count - 1), 0);
        }

        public INetPtr<T> RegisterType<T>(Action<T> disposer)
        {
            registeredTypes.Add(typeof(T));
            return new NetPtr<T>((ushort)(registeredTypes.Count - 1), 0);
        }

        public INetPtr<T> RegisterType<T>(Func<T> instantiator, Action<T> disposer)
        {
            registeredTypes.Add(typeof(T));
            return new NetPtr<T>((ushort)(registeredTypes.Count - 1), 0);
        }

        public INetPtr<T> RegisterType<T>(ushort explicitId, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            registeredTypes.Add(typeof(T));
            return new NetPtr<T>(explicitId, 0);
        }

        public INetPtr<T> RegisterType<T>(ushort explicitId, Func<T> instantiator, Action<T> disposer)
        {
            registeredTypes.Add(typeof(T));
            return new NetPtr<T>(explicitId, 0);
        }

        public INetPtr<T> RegisterType<T>(ushort explicitId, Func<T> instantiator, Action<T> disposer, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            registeredTypes.Add(typeof(T));
            return new NetPtr<T>(explicitId, 0);
        }

        public INetPtr<T> RegisterType<T>(ushort explicitId, Func<T> instantiator)
        {
            registeredTypes.Add(typeof(T));
            return new NetPtr<T>(explicitId, 0);
        }

        public bool TryGetAmbiguousSerializableType(INetPtr id, out ISerializableNetworkType serializableNetworkType)
        {
            serializableNetworkType = network;
            return true;
        }

        public bool TryGetAmbiguousType(INetPtr ptr, out INetworkType type)
        {
            type = network;
            return true;
        }

        public bool TryGetSerializableType<T>(out ISerializableNetworkType<T> serializableNetworkType)
        {
            if (network is ISerializableNetworkType<T> sType)
            {
                serializableNetworkType = sType;
                return true;
            }
            serializableNetworkType = default;
            return false;
        }

        public bool TryGetSerializableType<T>(INetPtr<T> id, out ISerializableNetworkType<T> serializableNetworkType)
        {
            if (network is ISerializableNetworkType<T> sType)
            {
                serializableNetworkType = sType;
                return true;
            }
            serializableNetworkType = default;
            return false;
        }

        public bool TryGetType<T>(INetPtr<T> ptr, out INetworkType<T> type)
        {
            if (networkType is INetworkType<T> t)
            {
                type = t;
                return true;
            }
            type = default;
            return true;
        }

        public bool TryGetType<T>(out INetworkType<T> type)
        {
            if (networkType is INetworkType<T> t)
            {
                type = t;
                return true;
            }
            type = default;
            return true;
        }

        public bool TryGetTypePtr<T>(out INetPtr<T> ptr)
        {
            ptr = new NetPtr<T>(1, 0); ;
            return true;
        }

        public bool TryGetTypePtr(Type type, out INetPtr ptr)
        {
            ptr = new NetPtr(1, 0); ;
            return true;
        }
    }
}

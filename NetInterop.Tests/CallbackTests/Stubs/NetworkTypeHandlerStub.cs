using NetInterop.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Tests.CallbackTests.Stubs
{
    public class NetworkTypeHandlerStub : INetTypeHandler
    {
        public List<Type> registeredTypes = new List<Type>();
        public bool cleared = false;
        public ISerializableNetType network = null;
        public INetType networkType = null;

        public INetPtr<T> RegisterType<T>(ushort interopId, IActivator<T> activator, IDeactivator<T> deactivator, IPacketSerializer<T> serializer, IPacketDeserializer<T> deserializer)
        {
            throw new NotImplementedException();
        }

        public bool TryGetSerializableType<T>(out ISerializableNetType<T> netType)
        {
            netType = (ISerializableNetType<T>)network;
            return true;
        }

        public bool TryGetSerializableType(Type type, out ISerializableNetType netType)
        {
            netType = network;
            return true;
        }

        public bool TryGetSerializableType(INetPtr typePtr, out ISerializableNetType netType)
        {
            netType = network;
            return true;
        }

        public bool TryGetSerializableType<T>(INetPtr typePtr, out ISerializableNetType<T> netType)
        {
            netType = (ISerializableNetType<T>)network;
            return true;
        }

        public bool TryGetType<T>(out INetType<T> netType)
        {
            netType = (INetType<T>)networkType;
            return true;
        }

        public bool TryGetType(Type type, out INetType netType)
        {
            netType = networkType;
            return true;
        }

        public bool TryGetType(INetPtr typePtr, out INetType netType)
        {
            netType = networkType;
            return true;
        }

        public bool TryGetType<T>(INetPtr typePtr, out INetType<T> netType)
        {
            netType = (INetType<T>)networkType;
            return true;
        }
    }
}

using NetInterop.Transport.Core.Abstractions.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop
{
    public class NetworkCallbackRegistrar<TPacket> : INetworkCallbackRegistrar<TPacket> where TPacket : Enum, IConvertible
    {
        private readonly IDictionary<ushort, Action<bool, IPacket<TPacket>>> callbacks = new ConcurrentDictionary<ushort, Action<bool, IPacket<TPacket>>>();
        private ushort nextId = 0;
        private readonly ConcurrentBag<ushort> freedIds = new ConcurrentBag<ushort>();

        public ushort Register(Action callback) => Register((goodResponse, packet) => callback());

        public ushort Register(Action<bool> callback) => Register((goodResponse, packet) => callback(goodResponse));

        public ushort Register(Action<bool, IPacket<TPacket>> callback)
        {
            ushort id = GetId();

            callbacks.Add(id, callback);

            return id;
        }

        public void InvokeAndFree(ushort id, bool response, IPacket<TPacket> packet)
        {
            if (callbacks.ContainsKey(id))
            {
                var invokable = callbacks[id];

                FreeId(id);

                invokable(response, packet);
            }
        }

        public void Invoke(ushort id, bool response, IPacket<TPacket> packet)
        {
            if (callbacks.ContainsKey(id))
            {
                var invokable = callbacks[id];

                invokable(response, packet);
            }
        }

        public void FreeId(ushort id)
        {
            freedIds.Add(id);
            callbacks.Remove(id);
        }

        private ushort GetId()
        {
            if (freedIds.IsEmpty)
            {
                return nextId += (ushort)1;
            }

            if (freedIds.TryTake(out ushort newId))
            {
                return newId;
            }

            return nextId += (ushort)1;
        }
    }
}

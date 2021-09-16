using RemoteInvoke.Net.Transport.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace RemoteInvoke.Net.Transport.Packets.Converters
{
    public class StructConverter<TPacketType> : IStructPacketConverter<TPacketType> where TPacketType : Enum, IConvertible
    {
        private readonly IPrimitivePacketConverter<TPacketType> primitiveConverter;

        public StructConverter(IPrimitivePacketConverter<TPacketType> primitiveConverter)
        {
            this.primitiveConverter = primitiveConverter;
        }

        public Packet<TPacketType> Convert<T>(T value) where T : unmanaged
        {
            // we know that if the object is unmanaged then all the types that it contains are also unmanaged
            // so either we use primitive packet converter or recurse to this method to pack the type
            var members = typeof(T).GetMembers(BindingFlags.Public | BindingFlags.Instance);

            foreach (var item in members)
            {
                // only serialize public fields and properties
                if ((item.MemberType & (MemberTypes.Field | MemberTypes.Property)) != 0)
                {

                }
            }

            return default;
        }
    }
}

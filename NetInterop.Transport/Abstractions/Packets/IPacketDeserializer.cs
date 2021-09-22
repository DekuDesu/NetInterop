using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Transport.Core.Abstractions.Packets
{
    /// <summary>
    /// Defines an object capable of deserializing a <see cref="IPacket{TPacket}"/> into a <typeparamref name="TResult"/>
    /// </summary>
    /// <typeparam name="TPacket"></typeparam>
    /// <typeparam name="TResult"></typeparam>
    public interface IPacketDeserializer<TPacket, TResult> : IPacketDeserializer<TPacket> where TPacket : Enum, IConvertible
    {
        TResult Deserialize(IPacket<TPacket> packet);
    }
    public interface IPacketDeserializer<TPacket> where TPacket : Enum, IConvertible
    {
        object AmbiguousDeserialize(IPacket<TPacket> packet);
    }
}

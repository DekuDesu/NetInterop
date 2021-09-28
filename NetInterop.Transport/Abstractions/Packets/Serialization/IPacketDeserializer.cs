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
    public interface IPacketDeserializer<TResult> : IPacketDeserializer
    {
        TResult Deserialize(IPacket packet);
    }
    public interface IPacketDeserializer
    {
        object AmbiguousDeserialize(IPacket packet);
    }
}

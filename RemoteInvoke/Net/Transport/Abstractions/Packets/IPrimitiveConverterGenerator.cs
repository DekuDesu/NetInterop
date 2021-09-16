using System;

namespace RemoteInvoke.Net.Transport.Packets
{
    public interface IPrimitiveConverterGenerator
    {
        ToSpanFunc<object, byte> GetPrimitiveConverter(Type type);
    }
}
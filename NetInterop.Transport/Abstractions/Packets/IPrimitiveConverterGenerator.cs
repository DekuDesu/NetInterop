using System;
using NetInterop.Transport.Core.Delegates;
namespace NetInterop.Transport.Core.Abstractions.Packets
{
    public interface IPrimitiveConverterGenerator
    {
        ToSpanFunc<object, byte> GetPrimitiveConverter(Type type);
    }
}
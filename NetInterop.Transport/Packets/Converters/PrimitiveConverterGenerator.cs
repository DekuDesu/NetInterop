using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NetInterop.Transport.Core.Delegates;
using NetInterop.Transport.Core.Abstractions;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets.Extensions;

namespace NetInterop.Transport.Core.Packets
{
    public class PrimitiveConverterGenerator : IPrimitiveConverterGenerator
    {
        /// <summary>
        /// Returns a delegate that when invoked will convert the given <typeparamref name="T"/> to a byte Span
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public ToSpanFunc<object, byte> GetPrimitiveConverter(Type type)
        {
            // this might have performance implications because of the boxing of objects on the heap
            // however, when compiled for specific types it may be optimized out by the compiler and avoid boxing TBD
            if (type.IsPrimitive)
            {
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean:
                        return (value) => ((bool)value).ToSpan();
                    case TypeCode.Char:
                        return (value) => ((ushort)value).ToSpan();
                    case TypeCode.SByte:
                        return (value) => ((sbyte)value).ToSpan();
                    case TypeCode.Byte:
                        return (value) => ((byte)value).ToSpan();
                    case TypeCode.Int16:
                        return (value) => ((short)value).ToSpan();
                    case TypeCode.UInt16:
                        return (value) => ((ushort)value).ToSpan();
                    case TypeCode.Int32:
                        return (value) => ((int)value).ToSpan();
                    case TypeCode.UInt32:
                        return (value) => ((uint)value).ToSpan();
                    case TypeCode.Int64:
                        return (value) => ((long)value).ToSpan();
                    case TypeCode.UInt64:
                        return (value) => ((ulong)value).ToSpan();
                    case TypeCode.Single:
                        return (value) => ((float)value).ToSpan();
                    case TypeCode.Double:
                        return (value) => ((double)value).ToSpan();
                    case TypeCode.Decimal:
                        return (value) => ((decimal)value).ToSpan();
                    case TypeCode.DateTime:
                        return (value) => ((DateTime)value).ToSpan();
                    case TypeCode.String:
                        return (value) => ((string)value).ToSpan(Encoding.UTF8);
                    case TypeCode.Empty:
                    case TypeCode.Object:
                    case TypeCode.DBNull:
                        break;
                }
            }
            throw new InvalidOperationException("Can not convert a non-primitive unmanaged type to a primitive Span<byte> converter");
        }
    }
}

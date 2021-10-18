using NetInterop.Abstractions;
using NetInterop.Example.Serializers;
using NetInterop.Runtime.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Example
{
    public static class Startup
    {
        private static ushort nextTypeId;

        public static void Initialize()
        {
            Interop.Initialize();

            RegisterTypes(Interop.Types, ref nextTypeId);

            RegisterMethods(Interop.Methods);
        }

        public static void RegisterTypes(ITypeHandler handler, ref ushort nextTypeId)
        {
            handler.RegisterPrimitiveTypes();

            handler.RegisterType<string>((ushort)TypeCode.String, UTF8Serializer.Instance, UTF8Serializer.Instance);

            DiagnosticClass.RegisterType(handler, ref nextTypeId);
        }

        public static void RegisterMethods(IMethodHandler handler)
        {
            DiagnosticClass.RegisterMethods(handler);
        }
    }
}

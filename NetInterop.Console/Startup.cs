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
        private static readonly IAddressProvider addressProvider = new DefaultAddressProvider();

        public static void Initialize()
        {
            Interop.Initialize();

            RegisterTypes(Interop.Types, addressProvider);
            RegisterMethods(Interop.Methods);
        }

        public static void RegisterTypes(ITypeHandler handler, IAddressProvider provider)
        {
            handler.RegisterPrimitiveTypes(provider);

            handler.RegisterType<string>(provider.GetNewAddress(), UTF8Serializer.Instance, UTF8Serializer.Instance);

            HelloWorld.RegisterType(handler, provider);
        }

        public static void RegisterMethods(IMethodHandler handler)
        {
            HelloWorld.RegisterMethods(handler);
        }
    }
}

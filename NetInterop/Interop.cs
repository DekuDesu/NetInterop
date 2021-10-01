using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using NetInterop.Clients;
using NetInterop.Servers;

namespace NetInterop
{
    public static class Interop
    {
        public static IClient CreateClient()
        {
            return new InteropClient();
        }
        public static IServer CreateServer()
        {
            return new InteropServer();
        }
    }
}

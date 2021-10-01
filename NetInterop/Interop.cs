using NetInterop.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using NetInterop.Clients;

namespace NetInterop
{
    public static class Interop
    {
        public static IClient CreateClient()
        {
            return new InteropClient();
        }
    }
}

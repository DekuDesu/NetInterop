using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NetInterop.Transport.Helpers
{
    public static class Networking
    {
        public static IPAddress GetLocalhost()
        {
            return IPAddress.Parse("127.0.0.1");
        }
    }
}

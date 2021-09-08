using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.Exceptions
{
    public class NullNetworkPointerException : NullReferenceException
    {
        public NullNetworkPointerException(string message) : base($"Attempted to reference null network pointer. {message}")
        {
        }
    }
}

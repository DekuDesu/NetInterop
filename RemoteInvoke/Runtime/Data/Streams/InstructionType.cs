using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteInvoke.Runtime.Data
{
    public enum InstructionType
    {
        none,
        /// <summary>
        /// Requests the receiving server/client to send a ping response
        /// </summary>
        ping
    }
}

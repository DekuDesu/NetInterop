using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using RemoteInvoke.Runtime;

namespace RemoteInvoke.Interop
{
    public class RemoteMarshalled<T>
    {
        private readonly IPointerCollection managedPointers;

        public RemoteMarshalled(IPointerCollection managedPointers)
        {
            this.managedPointers = managedPointers;
        }

        public int Ptr { get; private set; }

        public T Value => managedPointers.Read<T>(Ptr);


    }
}

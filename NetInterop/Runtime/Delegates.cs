using System;
using System.Collections.Generic;
using System.Text;

namespace NetInterop.Runtime
{
    public delegate void RefAction<T>(ref T value);
}

using RemoteInvoke.Runtime;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
namespace RemoteInvokeTests.Runtime
{
    public class PointerDictionaryTests
    {
        [Fact]
        public void Test_Count()
        {
            IPointerCollection pointers = new LocalPointerCollection();

            Assert.Equal(0, pointers.Count);

            pointers.Allocate(12);

            Assert.Equal(1, pointers.Count);
        }

        [Fact]
        public void Test_Resize()
        {
            IPointerCollection pointers = new LocalPointerCollection();

            Assert.Equal(0, pointers.Count);

            for (int i = 0; i < 1000; i++)
            {
                pointers.Allocate(12);
            }

            Assert.Equal(1000, pointers.Count);

            Assert.Equal(12, pointers.Read<int>(1000));
        }

        [Fact]
        public void Test_AllocFree()
        {
            IPointerCollection pointers = new LocalPointerCollection();

            int ptr = pointers.Allocate(14);

            Assert.Equal(14, pointers.Read<int>(ptr));

            pointers.Free(ptr);

            Assert.Null(pointers.Read<object>(ptr));

            int newptr = pointers.Allocate(22);

            Assert.Equal(ptr, newptr);

            Assert.Equal(22, pointers.Read<int>(ptr));
        }

        [Fact]
        public void Test_Write()
        {
            IPointerCollection pointers = new LocalPointerCollection();

            int ptr = pointers.Allocate(14);

            Assert.Equal(14, pointers.Read<int>(ptr));

            pointers.Write(ptr, 12);

            Assert.Equal(12, pointers.Read<int>(ptr));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NetInterop;

namespace NetInterop.Tests.Helpers
{
    public class GuidsTests
    {
        [Fact]
        public void Test_ZeroExtendIntoToGuidHelper()
        {
            Guid expected = Guid.Parse("{0FFFFFFF-0000-0000-0000-000000000000}");

            Guid actual = NetInterop.Helpers.Guids.ToGuid(0x0FFF_FFFF); // signed int 

            Assert.Equal(expected, actual);
        }
    }
}

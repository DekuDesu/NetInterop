using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RemoteInvoke.Runtime.Data;

namespace RemoteInvokeTests.Runtime
{
    public class HeaderTests
    {
        [Theory]
        [InlineData("1000001000000000000001000", 1024, 8)] // random packet size
        [InlineData("1111111111111111111111111", 65535, 255)] // largest small packet
        [InlineData("1000000000000000000000000", 0, 0)] // smallest small packet
        [InlineData("11111111111111111111111111111111", 65535 * 127, 255)] // largest packet
        public void Test_Header(string headerString, int expectedSize, int expectedType)
        {
            // convert string to uint
            uint header = Convert.ToUInt32(headerString, 2);

            IHeaderParser parser = new DefaultHeader();

            int length = parser.GetPacketType(header);

            Assert.Equal(expectedSize, length);

            int type = parser.GetPacketSize(header);

            Assert.Equal(expectedType, type);
        }
    }
}

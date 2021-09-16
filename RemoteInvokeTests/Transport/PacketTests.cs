using RemoteInvoke.Net.Transport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using RemoteInvoke.Net.Transport.Packets.Extensions;

namespace RemoteInvokeTests.Transport
{
    public class PacketTests
    {
        [Fact]
        public void Test_PacketCreate()
        {
            Packet<TypeCode> packet = Packet.Create(TypeCode.Byte);

            Assert.Equal(TypeCode.Byte, packet.PacketType);
        }

        [Fact]
        public void Test_PacketInitialize()
        {
            Packet<TypeCode> packet = new(TypeCode.Byte, 10);

            Assert.Equal(TypeCode.Byte, packet.PacketType);
            Assert.Equal(10, packet.Length);

            // the backing span has 4 bytes reserved for the packet header, this reduces allocations when we send the packet to the server
            // these 4 bytes are hidden from getting and setting other than using the dedicated methods
            Assert.Equal(14, packet.Data.Length);
            Assert.Equal(0, packet.EndOffset);
        }

        [Fact]
        public void Test_PacketAppend()
        {
            Packet<TypeCode> packet = new(TypeCode.Byte, 10);

            Assert.Equal(10, packet.Length);
            Assert.Equal(0, packet.EndOffset);

            // when we append data the position should increment by the length of data appended
            packet.Append(new byte[] { 1, 2, 3 });

            // since the amount of data we appended was less than the array size we should verify a new buffer was not allocated
            Assert.Equal(10, packet.Length);
            Assert.Equal(3, packet.EndOffset);
            Assert.Equal(1, packet.Data[0 + sizeof(uint)]);
            Assert.Equal(2, packet.Data[1 + sizeof(uint)]);
            Assert.Equal(3, packet.Data[2 + sizeof(uint)]);

            // append data to the END of the buffer but not exceeding make sure the buffer again does not extend
            packet.Append(new byte[] { 4, 5, 6, 7, 8, 9, 10 });

            Assert.Equal(10, packet.Length);
            Assert.Equal(10, packet.EndOffset);

            Assert.Equal(1, packet.Data[0 + sizeof(uint)]);
            Assert.Equal(2, packet.Data[1 + sizeof(uint)]);
            Assert.Equal(3, packet.Data[2 + sizeof(uint)]);
            Assert.Equal(4, packet.Data[3 + sizeof(uint)]);
            Assert.Equal(5, packet.Data[4 + sizeof(uint)]);
            Assert.Equal(6, packet.Data[5 + sizeof(uint)]);
            Assert.Equal(7, packet.Data[6 + sizeof(uint)]);
            Assert.Equal(8, packet.Data[7 + sizeof(uint)]);
            Assert.Equal(9, packet.Data[8 + sizeof(uint)]);
            Assert.Equal(10, packet.Data[9 + sizeof(uint)]);

            // append more than the backing array's size and ensure the backing array was resized and existing data was copied over
            packet.Append(new byte[] { 11 });

            Assert.Equal(11, packet.Length);
            Assert.Equal(11, packet.EndOffset);

            Assert.Equal(1, packet.Data[0 + sizeof(uint)]);
            Assert.Equal(2, packet.Data[1 + sizeof(uint)]);
            Assert.Equal(3, packet.Data[2 + sizeof(uint)]);
            Assert.Equal(4, packet.Data[3 + sizeof(uint)]);
            Assert.Equal(5, packet.Data[4 + sizeof(uint)]);
            Assert.Equal(6, packet.Data[5 + sizeof(uint)]);
            Assert.Equal(7, packet.Data[6 + sizeof(uint)]);
            Assert.Equal(8, packet.Data[7 + sizeof(uint)]);
            Assert.Equal(9, packet.Data[8 + sizeof(uint)]);
            Assert.Equal(10, packet.Data[9 + sizeof(uint)]);
            Assert.Equal(11, packet.Data[10 + sizeof(uint)]);
        }

        [Fact]
        public void Test_Remove()
        {
            Packet<TypeCode> packet = new(TypeCode.Byte, 10);

            Assert.Equal(10, packet.Length);
            Assert.Equal(0, packet.EndOffset);

            // when we append data the position should increment by the length of data appended
            packet.Append(new byte[] { 1, 2, 3 });

            // make sure the values we expect exist in their intended locations
            Assert.Equal(10, packet.Length);
            Assert.Equal(3, packet.EndOffset);
            Assert.Equal(1, packet.Data[0 + sizeof(uint)]);
            Assert.Equal(2, packet.Data[1 + sizeof(uint)]);
            Assert.Equal(3, packet.Data[2 + sizeof(uint)]);

            Span<byte> data = packet.Remove(3);

            Assert.Equal(3, packet.StartOffset);
            Assert.Equal(1, data[0]);
            Assert.Equal(2, data[1]);
            Assert.Equal(3, data[2]);


            // make sure the old data is still there, only internal state change should have been position
            Assert.Equal(1, packet.Data[0 + sizeof(uint)]);
            Assert.Equal(2, packet.Data[1 + sizeof(uint)]);
            Assert.Equal(3, packet.Data[2 + sizeof(uint)]);
        }

        [Fact]
        public void Test_Append_Array()
        {
            Packet<TypeCode> packet = new(TypeCode.Byte, 10);

            Assert.Equal(10, packet.Length);
            Assert.Equal(0, packet.EndOffset);

            int[] data = new int[] { 1, 2, 3, 4, 5 };

            packet.AppendArray(data);

            // 4 for the size, 4 for header, data * 4 for length
            Assert.Equal((data.Length * sizeof(int)) + 4 + sizeof(int), packet.ActualSize);


        }

        [Fact]
        public void Test_Get_Array()
        {
            Packet<TypeCode> packet = new(TypeCode.Byte, 10);

            Assert.Equal(10, packet.Length);
            Assert.Equal(0, packet.EndOffset);

            int[] data = new int[] { 1, 2, 3, 4, 5 };

            packet.AppendArray(data);

            // 4 for the size, 4 for header, data * 4 for length
            Assert.Equal((data.Length * sizeof(int)) + 4 + sizeof(int), packet.ActualSize);

            int[] actual = packet.GetIntArray();

            for (int i = 0; i < data.Length; i++)
            {
                Assert.True(data[i] == actual[i], $"Expected:\t{data[i]}\r\nActual:\t{actual[i]}\r\n\tat i={i}");
            }
        }
    }
}

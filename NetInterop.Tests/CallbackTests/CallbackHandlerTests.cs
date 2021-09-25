using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using NetInterop;
using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Tests.CallbackTests.Stubs;
using NetInterop.Transport.Core.Packets.Extensions;
using NetInterop.Transport.Core.Factories;
using NetInterop.Runtime.MethodHandling;

namespace NetInterop.Tests.CallbackTests
{
    public class CallbackHandlerTests
    {
        [Fact]
        public void Test_()
        {
            IPacketSerializer<int> intSerializer = new DefaultSerializer<int>((num, packet) => packet.AppendInt(num));
            IPacketDeserializer<int> intDeserializer = new DefaultDeserializer<int>((packet) => packet.GetInt());

            IPointerProvider pointerProvider = new DefaultPointerProvider();

            INetworkType<int> baseNetworkType = new DefaultNetworkType<int>(1, pointerProvider, () => 69);

            ISerializableNetworkType<int> networkType = new DefaultSerializableNetworkType<int>(baseNetworkType, intDeserializer, intSerializer);

            INetworkTypeHandler typeHandler = new NetworkTypeHandlerStub() { network = networkType, networkType = baseNetworkType };

            IPacketSender<TypeCode> sender = new PacketSenderStub<TypeCode>();

            IPointerResponseSender pointerSender = new ResponseSenderStub<TypeCode>(sender);

            IPacketHandler<PointerOperations> allocOperation = new AllocPointerHandler(typeHandler, pointerProvider, pointerSender);
            IPacketHandler<PointerOperations> setOperation = new SetPointerHandler(typeHandler, pointerProvider, pointerSender);
            IPacketHandler<PointerOperations> getOperation = new GetPointerHandler(typeHandler, pointerProvider, pointerSender);

            IPacketHandler operationHandler = new PointerPacketDispatchHandler<TypeCode>(allocOperation);

            IDelegateHandler<bool, IPacket> delegateHandler = new DefaultPacketDelegateHandler();

            // create some spots to put our actual results to compare
            bool ranCallback = false;
            IPacket callbackPacket = default;

            //  create a pointer packet
            IPacketSerializable pointer = pointerProvider.Create(1, 0);

            // wrap the pointer with a callback id
            IPacketSerializable<TypeCode> callback = new CallbackPacket<TypeCode>((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, delegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            IPacketSerializable<TypeCode> pointerOperation = new PointerOperationPacket<TypeCode>(PointerOperations.Alloc, callback);

            // send the packet to the server
            sender.Send(pointerOperation);

            // this is the packet as decoded by the server
            IPacket sent = ((PacketSenderStub<TypeCode>)sender).Sent.Dequeue();

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            operationHandler.Handle(sent);

            IPacket responsePacket = ((PacketSenderStub<TypeCode>)sender).Sent.Dequeue();

            Assert.NotNull(responsePacket);

            IPointerResponseHandler pointerCallbackHandler = new CallbackPacketHandler(delegateHandler);

            IPacketHandler<PointerOperations> repsonseHandler = new DefaultPointerReponseHandler(pointerCallbackHandler);

            repsonseHandler.Handle(responsePacket);

            Assert.True(ranCallback);
            Assert.NotNull(callbackPacket);

            Assert.Equal("0100", new NetPtr(callbackPacket.GetUShort(), callbackPacket.GetUShort()).ToString());
        }

        [Fact]
        public void Test_Set()
        {
            TestObjects<TypeCode> test = new();

            // create some spots to put our actual results to compare
            bool ranCallback = false;
            IPacket callbackPacket = default;

            //  create a pointer packet
            IPacketSerializable pointer = test.PointerProvider.Create(1, 0);

            // wrap the pointer with a callback id
            IPacketSerializable<TypeCode> callback = new CallbackPacket<TypeCode>((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, test.DelegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            IPacketSerializable<TypeCode> pointerOperation = new PointerOperationPacket<TypeCode>(PointerOperations.Alloc, callback);

            // send the packet to the server
            test.Sender.Send(pointerOperation);

            // this is the packet as decoded by the server
            IPacket sent = test.Receive();

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            test.OperationHandler.Handle(sent);

            IPacket responsePacket = test.Receive();

            Assert.NotNull(responsePacket);

            IPointerResponseHandler pointerCallbackHandler = new CallbackPacketHandler(test.DelegateHandler);

            IPacketHandler<PointerOperations> repsonseHandler = new DefaultPointerReponseHandler(pointerCallbackHandler);

            repsonseHandler.Handle(responsePacket);

            Assert.True(ranCallback);
            Assert.NotNull(callbackPacket);

            INetPtr<int> ptr = new NetPtr<int>(callbackPacket.GetUShort(), callbackPacket.GetUShort());

            Assert.Equal(0x01, ptr.PtrType);
            Assert.Equal(0x00, ptr.PtrAddress);

            // now we have alloced the value attempt to set it's value
            pointer = new SetPointerPacket<int>(ptr, 44, test.SerializableNetworkType);

            callback = new CallbackPacket<TypeCode>((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, test.DelegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            pointerOperation = new PointerOperationPacket<TypeCode>(PointerOperations.Set, callback);

            // send the packet to the server
            test.Send(pointerOperation);

            // this is the packet as decoded by the server
            sent = test.Receive();

            // reset the test results so we can check to see if the set worked
            ranCallback = false;
            callbackPacket = default;

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            test.OperationHandler.Handle(sent);

            responsePacket = test.Receive();

            Assert.NotNull(responsePacket);

            repsonseHandler.Handle(responsePacket);

            Assert.True(ranCallback);
            Assert.NotNull(callbackPacket);

            Assert.Equal(44, test.NetworkType.GetPtr(ptr));
        }

        [Fact]
        public void Test_Get()
        {
            TestObjects<TypeCode> test = new();

            INetPtr<int> ptr = test.NetworkType.AllocPtr().As<int>();

            test.NetworkType.SetPtr(ptr, 32);

            // create some spots to put our actual results to compare
            bool ranCallback = false;
            IPacket callbackPacket = default;

            // now we have alloced the value attempt to set it's value
            IPacketSerializable pointer = ptr;

            IPacketSerializable<TypeCode> callback = new CallbackPacket<TypeCode>((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, test.DelegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            IPacketSerializable<TypeCode> pointerOperation = new PointerOperationPacket<TypeCode>(PointerOperations.Get, callback);

            // send the packet to the server
            test.Send(pointerOperation);

            // this is the packet as decoded by the server
            IPacket sent = test.Receive();

            // reset the test results so we can check to see if the set worked
            ranCallback = false;
            callbackPacket = default;

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            test.OperationHandler.Handle(sent);

            IPacket responsePacket = test.Receive();

            Assert.NotNull(responsePacket);

            test.RepsonseHandler.Handle(responsePacket);

            Assert.True(ranCallback);
            Assert.NotNull(callbackPacket);

            Assert.Equal(32, responsePacket.GetInt());
        }

        [Fact]
        public void Test_Free()
        {
            TestObjects<TypeCode> test = new();

            INetPtr<int> ptr = test.NetworkType.AllocPtr().As<int>();

            test.NetworkType.SetPtr(ptr, 32);

            // create some spots to put our actual results to compare
            bool ranCallback = false;
            IPacket callbackPacket = default;

            // now we have alloced the value attempt to set it's value
            IPacketSerializable pointer = ptr;

            IPacketSerializable<TypeCode> callback = new CallbackPacket<TypeCode>((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, test.DelegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            IPacketSerializable<TypeCode> pointerOperation = new PointerOperationPacket<TypeCode>(PointerOperations.Free, callback);

            // send the packet to the server
            test.Send(pointerOperation);

            // this is the packet as decoded by the server
            IPacket sent = test.Receive();

            // reset the test results so we can check to see if the set worked
            ranCallback = false;
            callbackPacket = default;

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            test.OperationHandler.Handle(sent);

            IPacket responsePacket = test.Receive();

            Assert.NotNull(responsePacket);

            test.RepsonseHandler.Handle(responsePacket);

            Assert.True(ranCallback);
            Assert.NotNull(callbackPacket);

            // becuase int is a value type "freeing" it is just forgeting it's there, write over the value and check to see if the value was freed
            var newPtr = test.NetworkType.AllocPtr().As<int>();

            Assert.Equal(newPtr, ptr);

            // set the value of the new ptr and then check the old pointer and verify that we wrote over the value at the address of the old pointer
            test.NetworkType.SetPtr(newPtr, 0);

            Assert.Equal(0, test.NetworkType.GetPtr(ptr));
        }

        [Fact]
        public void Test_GetNetworkHeap()
        {

            TestObjects<TypeCode> test = new();

            INetworkHeap heap = new NetworkHeap<TypeCode>(test.TypeHandler, test.Sender, test.DelegateHandler, test.MethodHandler);

            INetPtr<int> ptr = test.NetworkType.AllocPtr().As<int>();

            test.NetworkType.SetPtr(ptr, 32);

            // now we have alloced the value attempt to set it's value
            IPacketSerializable pointer = ptr;

            int value = 0;

            heap.Get(ptr, (val) => value = val);

            // this is the packet as decoded by the server
            IPacket sent = test.Receive();

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            test.OperationHandler.Handle(sent);

            IPacket responsePacket = test.Receive();

            Assert.NotNull(responsePacket);

            test.RepsonseHandler.Handle(responsePacket);

            Assert.Equal(32, value);
        }

        [Fact]
        public void Test_SetNetworkHeap()
        {
            TestObjects<TypeCode> test = new();

            INetworkHeap heap = new NetworkHeap<TypeCode>(test.TypeHandler, test.Sender, test.DelegateHandler, test.MethodHandler);

            // get a ptr to set the value of
            INetPtr<int> ptr = test.NetworkType.AllocPtr().As<int>();

            heap.Set<int>(ptr, 22);

            // this is the packet as decoded by the server
            IPacket sent = test.Receive();

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            test.OperationHandler.Handle(sent);

            IPacket responsePacket = test.Receive();

            Assert.NotNull(responsePacket);

            test.RepsonseHandler.Handle(responsePacket);

            Assert.NotNull(responsePacket);

            Assert.Equal(22, test.NetworkType.GetPtr(ptr));
        }

        [Fact]
        public void Test_FreeNetworkHeap()
        {
            TestObjects<TypeCode> test = new();

            INetworkHeap heap = new NetworkHeap<TypeCode>(test.TypeHandler, test.Sender, test.DelegateHandler, test.MethodHandler);

            INetPtr<int> ptr = test.NetworkType.AllocPtr().As<int>();

            test.NetworkType.SetPtr(ptr, 32);

            heap.Destroy(ptr);

            // this is the packet as decoded by the server
            IPacket sent = test.Receive();

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            test.OperationHandler.Handle(sent);

            IPacket responsePacket = test.Receive();

            Assert.NotNull(responsePacket);

            test.RepsonseHandler.Handle(responsePacket);

            // becuase int is a value type "freeing" it is just forgeting it's there, write over the value and check to see if the value was freed
            var newPtr = test.NetworkType.AllocPtr().As<int>();

            Assert.Equal(newPtr, ptr);

            // set the value of the new ptr and then check the old pointer and verify that we wrote over the value at the address of the old pointer
            test.NetworkType.SetPtr(newPtr, 0);

            Assert.Equal(0, test.NetworkType.GetPtr(ptr));
        }

        [Fact]
        public void Test_AllocNetworkHeap()
        {
            TestObjects<TypeCode> test = new();

            INetworkHeap heap = new NetworkHeap<TypeCode>(test.TypeHandler, test.Sender, test.DelegateHandler, test.MethodHandler);

            INetPtr<int> ptr = default;

            heap.Create<int>((resultPtr) => ptr = resultPtr);

            // this is the packet as decoded by the server
            IPacket sent = test.Receive();

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            test.OperationHandler.Handle(sent);

            IPacket responsePacket = test.Receive();

            Assert.NotNull(responsePacket);

            test.RepsonseHandler.Handle(responsePacket);

            // first ptr should be 0100
            Assert.Equal("0100", ptr.ToString());
        }

        [Fact]
        public void Test_VerifySetup()
        {
            // we should make sure our test method itself is not flawed
            IPacketSender<TypeCode> sender = new PacketSenderStub<TypeCode>();

            IPacketSerializable<TypeCode> packet = new TestPacket() { Value = 12 };

            sender.Send(packet);

            IPacket sent = ((PacketSenderStub<TypeCode>)sender).Sent.Dequeue();

            Assert.Equal(12, sent.GetInt());

            Assert.Throws<IndexOutOfRangeException>(() => sent.GetByte());
        }

        [Fact]
        public void CallbackDelegateHandlerImplementationWorks()
        {
            IDelegateHandler<bool, IPacket> delegateHandler = new DefaultPacketDelegateHandler();

            bool response = false;
            IPacket packet = default;

            ushort id = delegateHandler.Register((arg0, arg1) =>
            {
                response = arg0;
                packet = arg1;
            });

            Assert.Equal(1, delegateHandler.Count);

            IPacket arg = Packet.Create(TypeCode.Boolean);

            arg.AppendInt(44);

            delegateHandler.Invoke(id, true, arg);

            Assert.True(response);
            Assert.NotNull(packet);
            Assert.Equal(44, packet.GetInt());
            Assert.Equal(0, delegateHandler.Count);
        }

        [Fact]
        public void Test_CallbackAppendingOrder()
        {
            IDelegateHandler<bool, IPacket> delegateHandler = new DefaultPacketDelegateHandler();
            // we should make sure our test method itself is not flawed
            IPacketSender<TypeCode> sender = new PacketSenderStub<TypeCode>();

            IPointerResponseSender responseSender = new DefaultPointerResponseSender<TypeCode>(sender);

            IPacketSerializable<TypeCode> packet = new TestPacket() { Value = 12 };

            bool invokedCallback = false;
            IPacket receivedPacket = default;

            // perform data appending
            IPacketSerializable<TypeCode> callbackWrappedPacket = new CallbackPacket<TypeCode>((goodResponse, packet) => { invokedCallback = true; receivedPacket = packet; }, packet, delegateHandler);

            // check results
            sender.Send(callbackWrappedPacket);

            IPacket sent = ((PacketSenderStub<TypeCode>)sender).Sent.Dequeue();

            ushort id = sent.GetUShort();

            Assert.Equal(1, id);

            delegateHandler.Invoke(id, true, sent);

            Assert.True(invokedCallback);
            Assert.NotNull(receivedPacket);
            Assert.Equal(sent, receivedPacket);

            Assert.Equal(12, receivedPacket.GetInt());
        }

        public class TestPacket : IPacketSerializable<TypeCode>
        {
            public int Value { get; set; }
            public TypeCode PacketType { get; } = TypeCode.Byte;

            public int EstimatePacketSize() => sizeof(int);

            public void Serialize(IPacket packetBuilder)
            {
                packetBuilder.AppendInt(Value);
            }
        }

        private class TestObjects<TPacket> where TPacket : Enum, IConvertible
        {
            public IPacketSerializer<int> IntSerializer { get; set; }
            public IPacketDeserializer<int> IntDeserializer { get; set; }
            public IPointerProvider PointerProvider { get; set; }
            public INetworkType<int> NetworkType { get; set; }
            public ISerializableNetworkType<int> SerializableNetworkType { get; set; }
            public INetworkTypeHandler TypeHandler { get; set; }
            public IPacketSender<TPacket> Sender { get; set; }
            public IPointerResponseSender PointerSender { get; set; }
            public IPacketHandler<PointerOperations> AllocOperation { get; set; }
            public IPacketHandler<PointerOperations> SetOperation { get; set; }
            public IPacketHandler<PointerOperations> GetOperation { get; set; }
            public IPacketHandler<PointerOperations> FreeOperation { get; set; }
            public IPacketHandler<PointerOperations> InvokeOperation { get; set; }
            public IPacketHandler OperationHandler { get; set; }
            public IDelegateHandler<bool, IPacket> DelegateHandler { get; set; }
            public IPointerResponseHandler PointerCallbackHandler { get; set; }
            public IPacketHandler<PointerOperations> RepsonseHandler { get; set; }
            public INetworkMethodHandler MethodHandler { get; set; }
            public TestObjects()
            {
                IntSerializer = new DefaultSerializer<int>((num, packet) => packet.AppendInt(num));
                IntDeserializer = new DefaultDeserializer<int>((packet) => packet.GetInt());

                PointerProvider = new DefaultPointerProvider();

                NetworkType = new DefaultNetworkType<int>(1, PointerProvider, () => 0);

                SerializableNetworkType = new DefaultSerializableNetworkType<int>(NetworkType, IntDeserializer, IntSerializer);

                TypeHandler = new NetworkTypeHandlerStub() { network = SerializableNetworkType, networkType = NetworkType };

                Sender = new PacketSenderStub<TPacket>();

                MethodHandler = new DefaultMethodHandler(PointerProvider, TypeHandler);

                PointerSender = new ResponseSenderStub<TPacket>(Sender);

                AllocOperation = new AllocPointerHandler(TypeHandler, PointerProvider, PointerSender);
                SetOperation = new SetPointerHandler(TypeHandler, PointerProvider, PointerSender);
                GetOperation = new GetPointerHandler(TypeHandler, PointerProvider, PointerSender);
                FreeOperation = new FreePointerHandler(TypeHandler, PointerProvider, PointerSender);
                InvokeOperation = new InvokePointerHandler(TypeHandler, PointerProvider, PointerSender, MethodHandler);

                OperationHandler = new PointerPacketDispatchHandler<TPacket>(AllocOperation, SetOperation, GetOperation, FreeOperation);

                DelegateHandler = new DefaultPacketDelegateHandler();

                PointerCallbackHandler = new CallbackPacketHandler(DelegateHandler);

                RepsonseHandler = new DefaultPointerReponseHandler(PointerCallbackHandler);
            }

            public IPacket Receive()
            {
                return ((PacketSenderStub<TPacket>)Sender).Sent.Dequeue();
            }
            public void Send(IPacketSerializable<TPacket> packet)
            {
                Sender.Send(packet);
            }
        }
    }
}

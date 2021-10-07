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
using NetInterop.Runtime.TypeHandling;
using NetInterop.Abstractions;
using NetInterop.Runtime;

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

            IType<int> baseNetworkType = new NetType<int>(pointerProvider.Create(1, 0), new DefaultActivator<int>(), new DefaultDeactivator<int>());

            ISerializableType<int> networkType = new SerializableNetType<int>(baseNetworkType, intSerializer, intDeserializer);

            ITypeHandler typeHandler = new NetworkTypeHandlerStub() { network = networkType, networkType = baseNetworkType };

            IPacketSender sender = new PacketSenderStub();

            IPointerResponseSender pointerSender = new ResponseSenderStub(sender);

            IObjectHeap heap = new RuntimeHeap(typeHandler, pointerProvider);

            IPacketHandler<PointerOperations> allocOperation = new AllocPointerHandler(heap, pointerProvider, pointerSender);
            IPacketHandler<PointerOperations> setOperation = new SetPointerHandler(heap, typeHandler, pointerProvider, pointerSender);
            IPacketHandler<PointerOperations> getOperation = new GetPointerHandler(heap, typeHandler, pointerProvider, pointerSender);

            IPacketHandler operationHandler = new PointerPacketDispatchHandler(allocOperation);

            IDelegateHandler<bool, IPacket> delegateHandler = new DefaultPacketDelegateHandler();

            // create some spots to put our actual results to compare
            bool ranCallback = false;
            IPacket callbackPacket = default;

            //  create a pointer packet
            IPacketSerializable pointer = pointerProvider.Create(1, 0);

            // wrap the pointer with a callback id
            IPacketSerializable callback = new CallbackPacket((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, delegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            IPacketSerializable pointerOperation = new PointerOperationPacket(PointerOperations.Alloc, callback);

            // send the packet to the server
            sender.Send(pointerOperation);

            // this is the packet as decoded by the server
            IPacket sent = ((PacketSenderStub)sender).Sent.Dequeue();

            // sort the packet into the correct handler
            // this should route to Alloc handler and should alloc a new int ptr
            // it should then respond with a ptr back
            operationHandler.Handle(sent);

            IPacket responsePacket = ((PacketSenderStub)sender).Sent.Dequeue();

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
            TestObjects test = new();

            // create some spots to put our actual results to compare
            bool ranCallback = false;
            IPacket callbackPacket = default;

            //  create a pointer packet
            IPacketSerializable pointer = test.PointerProvider.Create(1, 0);

            // wrap the pointer with a callback id
            IPacketSerializable callback = new CallbackPacket((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, test.DelegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            IPacketSerializable pointerOperation = new PointerOperationPacket(PointerOperations.Alloc, callback);

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

            callback = new CallbackPacket((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, test.DelegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            pointerOperation = new PointerOperationPacket(PointerOperations.Set, callback);

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

            Assert.Equal(44, test.RuntimeHeap.Get(ptr));
        }

        [Fact]
        public void Test_Get()
        {
            TestObjects test = new();

            INetPtr<int> ptr = test.RuntimeHeap.Alloc(test.IntTypePointer).As<int>();

            test.RuntimeHeap.Set(ptr, 32);

            // create some spots to put our actual results to compare
            bool ranCallback = false;
            IPacket callbackPacket = default;

            // now we have alloced the value attempt to set it's value
            IPacketSerializable pointer = ptr;

            IPacketSerializable callback = new CallbackPacket((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, test.DelegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            IPacketSerializable pointerOperation = new PointerOperationPacket(PointerOperations.Get, callback);

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
            TestObjects test = new();

            INetPtr<int> ptr = test.RuntimeHeap.Alloc(test.IntTypePointer).As<int>();

            test.RuntimeHeap.Set(ptr, 32);

            // create some spots to put our actual results to compare
            bool ranCallback = false;
            IPacket callbackPacket = default;

            // now we have alloced the value attempt to set it's value
            IPacketSerializable pointer = ptr;

            IPacketSerializable callback = new CallbackPacket((x, y) => { ranCallback = x; callbackPacket = y; }, pointer, test.DelegateHandler);

            // wrap the callback with a operation code so it can be sorted to the correct handler by the serrver
            IPacketSerializable pointerOperation = new PointerOperationPacket(PointerOperations.Free, callback);

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
            var newPtr = test.RuntimeHeap.Alloc(test.IntTypePointer).As<int>();

            Assert.Equal(newPtr, ptr);

            // set the value of the new ptr and then check the old pointer and verify that we wrote over the value at the address of the old pointer
            test.RuntimeHeap.Set(newPtr, 0);

            Assert.Equal(0, test.RuntimeHeap.Get(ptr));
        }

        [Fact]
        public void Test_GetNetworkHeap()
        {

            TestObjects test = new();

            INetworkHeap heap = new NetworkHeap(test.TypeHandler, test.Sender, test.DelegateHandler, test.MethodHandler);

            INetPtr<int> ptr = test.RuntimeHeap.Alloc(test.IntTypePointer).As<int>();

            test.RuntimeHeap.Set(ptr, 32);

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
            TestObjects test = new();

            INetworkHeap heap = new NetworkHeap(test.TypeHandler, test.Sender, test.DelegateHandler, test.MethodHandler);

            // get a ptr to set the value of
            INetPtr<int> ptr = test.RuntimeHeap.Alloc(test.IntTypePointer).As<int>();

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

            Assert.Equal(22, test.RuntimeHeap.Get(ptr));
        }

        [Fact]
        public void Test_FreeNetworkHeap()
        {
            TestObjects test = new();

            INetworkHeap heap = new NetworkHeap(test.TypeHandler, test.Sender, test.DelegateHandler, test.MethodHandler);

            INetPtr<int> ptr = test.RuntimeHeap.Alloc(test.IntTypePointer).As<int>();

            test.RuntimeHeap.Set(ptr, 32);

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
            var newPtr = test.RuntimeHeap.Alloc(test.IntTypePointer).As<int>();

            Assert.Equal(newPtr, ptr);

            // set the value of the new ptr and then check the old pointer and verify that we wrote over the value at the address of the old pointer
            test.RuntimeHeap.Set(newPtr, 0);

            Assert.Equal(0, test.RuntimeHeap.Get(ptr));
        }

        [Fact]
        public void Test_AllocNetworkHeap()
        {
            TestObjects test = new();

            INetworkHeap heap = new NetworkHeap(test.TypeHandler, test.Sender, test.DelegateHandler, test.MethodHandler);

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
            IPacketSender sender = new PacketSenderStub();

            IPacketSerializable packet = new TestPacket() { Value = 12 };

            sender.Send(packet);

            IPacket sent = ((PacketSenderStub)sender).Sent.Dequeue();

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

            IPacket arg = Packet.Create(0);

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
            IPacketSender sender = new PacketSenderStub();

            IPointerResponseSender responseSender = new DefaultPointerResponseSender(sender);

            IPacketSerializable packet = new TestPacket() { Value = 12 };

            bool invokedCallback = false;
            IPacket receivedPacket = default;

            // perform data appending
            IPacketSerializable callbackWrappedPacket = new CallbackPacket((goodResponse, packet) => { invokedCallback = true; receivedPacket = packet; }, packet, delegateHandler);

            // check results
            sender.Send(callbackWrappedPacket);

            IPacket sent = ((PacketSenderStub)sender).Sent.Dequeue();

            ushort id = sent.GetUShort();

            Assert.Equal(1, id);

            delegateHandler.Invoke(id, true, sent);

            Assert.True(invokedCallback);
            Assert.NotNull(receivedPacket);
            Assert.Equal(sent, receivedPacket);

            Assert.Equal(12, receivedPacket.GetInt());
        }

        public class TestPacket : IPacketSerializable
        {
            public int Value { get; set; }

            public int EstimatePacketSize() => sizeof(int);

            public void Serialize(IPacket packetBuilder)
            {
                packetBuilder.AppendInt(Value);
            }
        }

        private class TestObjects
        {
            public IPacketSerializer<int> IntSerializer { get; set; }
            public IPacketDeserializer<int> IntDeserializer { get; set; }
            public IPointerProvider PointerProvider { get; set; }
            public IType<int> NetworkType { get; set; }
            public ISerializableType<int> SerializableNetworkType { get; set; }
            public ITypeHandler TypeHandler { get; set; }
            public IPacketSender Sender { get; set; }
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
            public IMethodHandler MethodHandler { get; set; }
            public IObjectHeap RuntimeHeap { get; set; }
            public INetPtr<int> IntTypePointer { get; set; }
            public TestObjects()
            {
                IntSerializer = new DefaultSerializer<int>((num, packet) => packet.AppendInt(num));
                IntDeserializer = new DefaultDeserializer<int>((packet) => packet.GetInt());

                PointerProvider = new DefaultPointerProvider();

                IntTypePointer = PointerProvider.Create(1, 0).As<int>();

                NetworkType = new NetType<int>(IntTypePointer, new DefaultActivator<int>(), new DefaultDeactivator<int>());

                SerializableNetworkType = new SerializableNetType<int>(NetworkType, IntSerializer, IntDeserializer);

                TypeHandler = new NetworkTypeHandlerStub() { network = SerializableNetworkType, networkType = NetworkType };
                
                RuntimeHeap = new RuntimeHeap(TypeHandler,PointerProvider);

                Sender = new PacketSenderStub();

                MethodHandler = new DefaultMethodHandler(PointerProvider, TypeHandler, RuntimeHeap);

                PointerSender = new ResponseSenderStub(Sender);


                AllocOperation = new AllocPointerHandler(RuntimeHeap, PointerProvider, PointerSender);
                SetOperation = new SetPointerHandler(RuntimeHeap, TypeHandler, PointerProvider, PointerSender);
                GetOperation = new GetPointerHandler(RuntimeHeap, TypeHandler, PointerProvider, PointerSender);
                FreeOperation = new FreePointerHandler(RuntimeHeap, PointerProvider, PointerSender);
                InvokeOperation = new InvokePointerHandler(PointerProvider, PointerSender, MethodHandler);

                OperationHandler = new PointerPacketDispatchHandler(AllocOperation, SetOperation, GetOperation, FreeOperation);

                DelegateHandler = new DefaultPacketDelegateHandler();

                PointerCallbackHandler = new CallbackPacketHandler(DelegateHandler);

                RepsonseHandler = new DefaultPointerReponseHandler(PointerCallbackHandler);
            }

            public IPacket Receive()
            {
                return ((PacketSenderStub)Sender).Sent.Dequeue();
            }
            public void Send(IPacketSerializable packet)
            {
                Sender.Send(packet);
            }
        }
    }
}

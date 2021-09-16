using NetInterop.Transport.Core.Abstractions.Packets;
using NetInterop.Transport.Core.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetInterop.Transport.Core.Packets
{
    public class DefaultPacketReceiver<TPacketType> : IPacketReceiver<TPacketType> where TPacketType : Enum, IConvertible
    {
        private readonly IPacketController<TPacketType> controller;
        private readonly IPacketDispatcher<TPacketType> dispatcher;

        public DefaultPacketReceiver(IPacketDispatcher<TPacketType> dispatcher, IPacketController<TPacketType> controller)
        {
            this.dispatcher = dispatcher ?? throw new ArgumentNullException(nameof(dispatcher));
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
        }

        public void BeginReceiving(CancellationToken token)
        {
            while (token.IsCancellationRequested is false)
            {
                Packet<TPacketType> packet = controller.WaitForPacket(token);

                dispatcher.Dispatch(ref packet);

                Thread.Sleep(1);
            }
        }
    }
}

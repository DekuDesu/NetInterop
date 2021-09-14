using RemoteInvoke.Net.Transport.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteInvoke.Net.Transport.Packets
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

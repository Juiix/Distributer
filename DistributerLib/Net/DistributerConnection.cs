using DistributerLib.Net.Handlers;
using DistributerLib.Net.Packets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Utils.NET.Net;
using Utils.NET.Net.Tcp;

namespace DistributerLib.Net
{
    public class DistributerConnection : NetConnection<DPacket>
    {
        public static int Port = 12321;

        public const string Packages_Path = "Packages";

        private PacketHandlerFactory<DistributerConnection, DPacketHandler<DPacket>, DPacket> _handlers = new PacketHandlerFactory<DistributerConnection, DPacketHandler<DPacket>, DPacket>();

        public DistributerConnection(Socket socket) : base(socket)
        {

        }

        public DistributerConnection() : base()
        {

        }

        public override void HandlePacket(DPacket packet)
        {
            _handlers.Handle(packet, this);
        }
    }
}

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
        public DistributerConnection(Socket socket, DPacketFactory packetFactory) : base(socket, packetFactory)
        {
            
        }
    }
}

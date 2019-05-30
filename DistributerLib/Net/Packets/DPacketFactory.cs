using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.Net;

namespace DistributerLib.Net.Packets
{
    public class DPacketFactory : PacketFactory<DPacket>
    {
        private DistributerConnection connection;

        public DPacketFactory(DistributerConnection connection)
        {
            this.connection = connection;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.Net;

namespace DistributerLib.Net.Packets
{
    public abstract class DPacket : Packet
    {
        public override byte Id => (byte)Type;

        public abstract DPacketType Type { get; }
    }
}

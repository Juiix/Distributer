using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.Net;

namespace DistributerLib.Net.Packets
{
    public abstract class DPacket : Packet, ITokenPacket
    {
        public override byte Id => (byte)Type;

        public abstract DPacketType Type { get; }

        public int Token { get; set; }

        public bool TokenResponse { get; set; }
    }
}

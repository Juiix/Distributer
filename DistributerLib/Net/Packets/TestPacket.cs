using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;

namespace DistributerLib.Net.Packets
{
    public class TestPacket : DPacket
    {
        public override DPacketType Type => DPacketType.Test;

        protected override void Read(BitReader r)
        {

        }

        protected override void Write(BitWriter w)
        {

        }
    }
}

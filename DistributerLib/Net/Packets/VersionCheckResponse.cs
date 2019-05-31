using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;
using Utils.NET.Net;

namespace DistributerLib.Net.Packets
{
    public class VersionCheckResponse : DPacket
    {
        public override DPacketType Type => DPacketType.VersionCheckResp;

        public bool needsUpdate;

        protected override void Read(BitReader r)
        {
            needsUpdate = r.ReadBool();
        }

        protected override void Write(BitWriter w)
        {
            w.Write(needsUpdate);
        }
    }
}

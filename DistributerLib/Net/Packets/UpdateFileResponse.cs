using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;

namespace DistributerLib.Net.Packets
{
    public class UpdateFileResponse : DPacket
    {
        public override DPacketType Type => DPacketType.UpdateFileResp;

        public bool success;

        protected override void Read(BitReader r)
        {
            success = r.ReadBool();
        }

        protected override void Write(BitWriter w)
        {
            w.Write(success);
        }
    }
}

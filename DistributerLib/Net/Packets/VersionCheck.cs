using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;
using Utils.NET.Net;

namespace DistributerLib.Net.Packets
{
    public class VersionCheck : DPacket
    {
        public override DPacketType Type => DPacketType.VersionCheck;

        public string packageName;
        public byte[] checksum;

        protected override void Read(BitReader r)
        {
            packageName = r.ReadUTF();
            checksum = r.ReadBytes(r.ReadInt32());
        }

        protected override void Write(BitWriter w)
        {
            w.Write(packageName);
            w.Write(checksum.Length);
            w.Write(checksum);
        }
    }
}

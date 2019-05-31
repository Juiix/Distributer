using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.IO;

namespace DistributerLib.Net.Packets
{
    public class UpdateFile : DPacket
    {
        public override DPacketType Type => DPacketType.UpdateFile;

        public string packageName;
        public byte[] file;
        public byte[] checksum;

        protected override void Read(BitReader r)
        {
            packageName = r.ReadUTF();
            file = r.ReadBytes(r.ReadInt32());
            checksum = r.ReadBytes(r.ReadInt32());
        }

        protected override void Write(BitWriter w)
        {
            w.Write(packageName);
            w.Write(file.Length);
            w.Write(file);
            w.Write(checksum.Length);
            w.Write(checksum);
        }
    }
}

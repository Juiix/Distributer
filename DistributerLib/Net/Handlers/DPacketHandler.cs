using DistributerLib.Net.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.Net;

namespace DistributerLib.Net.Handlers
{
    public abstract class DPacketHandler<TPacket> : IPacketHandler<DistributerConnection, DPacket> 
        where TPacket : DPacket
    {
        public byte Id => ((TPacket)Activator.CreateInstance(typeof(TPacket))).Id;

        public void Handle(DPacket packet, DistributerConnection connection)
        {
            HandlePacket((TPacket)packet, connection);
        }

        public abstract void HandlePacket(TPacket packet, DistributerConnection connection);
    }
}

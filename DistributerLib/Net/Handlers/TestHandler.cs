using DistributerLib.Net.Packets;
using System;
using System.Collections.Generic;
using System.Text;
using Utils.NET.Logging;

namespace DistributerLib.Net.Handlers
{
    public class TestHandler : DPacketHandler<TestPacket>
    {
        public override void HandlePacket(TestPacket packet, DistributerConnection connection)
        {

        }
    }
}

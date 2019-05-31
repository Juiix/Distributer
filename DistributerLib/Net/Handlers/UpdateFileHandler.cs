using DistributerLib.Net.Packets;
using System;
using System.Collections.Generic;
using System.Text;

namespace DistributerLib.Net.Handlers
{
    public class UpdateFileHandler : DPacketHandler<UpdateFile>
    {
        public override void HandlePacket(UpdateFile packet, DistributerConnection connection)
        {
            var response = new UpdateFileResponse();
            response.Token = packet.Token;


        }

        private bool ApplyUpdate(byte[] file, byte[] checksum)
        {

        }
    }
}

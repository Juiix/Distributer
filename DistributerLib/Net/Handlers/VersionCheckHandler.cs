using DistributerLib.Net.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Utils.NET.Logging;

namespace DistributerLib.Net.Handlers
{
    public class VersionCheckHandler : DPacketHandler<VersionCheck>
    {
        public override void HandlePacket(VersionCheck packet, DistributerConnection connection)
        {
            var response = new VersionCheckResponse();
            response.Token = packet.Token;
            response.needsUpdate = CheckNeedsUpdate(packet.packageName, packet.checksum); // TODO compare the checksum

            if (response.needsUpdate)
                Log.Write("This node requires an update", ConsoleColor.Green);
            else
                Log.Write("This node does not need to be updated", ConsoleColor.Cyan);

            connection.SendTokenResponse(response);
        }

        private bool CheckNeedsUpdate(string name, byte[] checksum)
        {
            string checksumPath = Path.Combine(DistributerConnection.Packages_Path, name, "checksum.md5");
            if (!File.Exists(checksumPath)) return true;

            var localChecksum = LoadChecksum(checksumPath);
            return !localChecksum.SequenceEqual(checksum);
        }

        private byte[] LoadChecksum(string path)
        {
            return File.ReadAllBytes(path);
        }
    }
}

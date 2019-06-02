using DistributerLib.Net.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DistributerLib.Net.Handlers
{
    public class UpdateFileHandler : DPacketHandler<UpdateFile>
    {
        public override void HandlePacket(UpdateFile packet, DistributerConnection connection)
        {
            var response = new UpdateFileResponse();
            response.Token = packet.Token;

            response.success = CacheUpdate(packet.packageName, packet.file, packet.checksum, connection);

            connection.SendTokenResponse(response);
        }

        private bool CacheUpdate(string packageName, byte[] file, byte[] checksum, DistributerConnection connection)
        {
            if (!GenerateChecksum(file).SequenceEqual(checksum)) return false; // file received was corrupted

            return connection.CacheUpdate(packageName, file);
        }

        /// <summary>
        /// Generates a checksum from the given bytes
        /// </summary>
        /// <returns>The checksum.</returns>
        /// <param name="file">File.</param>
        private byte[] GenerateChecksum(byte[] file)
        {
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(file);
            }
            return hash;
        }
    }
}

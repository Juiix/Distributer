using DistributerLib.Net.Handlers;
using DistributerLib.Net.Packets;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Net.Sockets;
using System.Text;
using Utils.NET.Net;
using Utils.NET.Net.Tcp;

namespace DistributerLib.Net
{
    public class DistributerConnection : NetConnection<DPacket>
    {
        public static int Port = 12321;

        public const string Packages_Path = "Packages";
        public const string Packages_Cache_Path = "Packages/.cache";

        private PacketHandlerFactory<DistributerConnection, DPacketHandler<DPacket>, DPacket> _handlers = new PacketHandlerFactory<DistributerConnection, DPacketHandler<DPacket>, DPacket>();

        public DistributerConnection(Socket socket) : base(socket)
        {

        }

        public DistributerConnection() : base()
        {

        }

        public override void HandlePacket(DPacket packet)
        {
            _handlers.Handle(packet, this);
        }

        public bool CacheUpdate(string packageName, byte[] file)
        {
            CreateDirectories();

            string packageCacheDirectory = Path.Combine(Packages_Cache_Path, packageName);
            string packageCacheFile = Path.Combine(Packages_Cache_Path, packageName + ".zip");

            if (Directory.Exists(packageCacheDirectory))
                Directory.Delete(packageCacheDirectory, true);
            Directory.CreateDirectory(packageCacheDirectory);

            File.WriteAllBytes(packageCacheFile, file);

            ZipFile.ExtractToDirectory(packageCacheFile, packageCacheDirectory);

            File.Delete(packageCacheFile);

            return true;
        }

        private void CreateDirectories()
        {
            if (!Directory.Exists(Packages_Path))
                Directory.CreateDirectory(Packages_Path);
            if (!Directory.Exists(Packages_Cache_Path))
            {
                var di = Directory.CreateDirectory(Packages_Cache_Path);
                di.Attributes |= FileAttributes.Hidden;
            }
        }
    }
}

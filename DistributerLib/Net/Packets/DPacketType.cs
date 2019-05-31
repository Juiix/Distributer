using System;
using System.Collections.Generic;
using System.Text;

namespace DistributerLib.Net.Packets
{
    public enum DPacketType : byte
    {
        None = 0,
        VersionCheck = 1,
        VersionCheckResp = 2,
        UpdateFile = 3,
        UpdateFileResp = 4
    }
}

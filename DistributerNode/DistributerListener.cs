using DistributerLib.Net;
using DistributerLib.Net.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils.NET.Logging;
using Utils.NET.Net.Tcp;

namespace DistributerNode
{
    public class DistributerListener : NetListener<DistributerConnection, DPacket>
    {
        private ConcurrentDictionary<NetConnection<DPacket>, DistributerConnection> connections = new ConcurrentDictionary<NetConnection<DPacket>, DistributerConnection>();

        public DistributerListener() : base(DistributerConnection.Port)
        {

        }

        public override void HandleConnection(DistributerConnection connection)
        {
            connection.SetDisconnectCallback(HandleDisconnection);
            if (connection.Disconnected) return;
            connections.TryAdd(connection, connection);
            Log.Write("Received distributer connection", ConsoleColor.Green);
            connection.ReadAsync();
        }

        private void HandleDisconnection(NetConnection<DPacket> net)
        {
            if (!connections.TryRemove(net, out var connection)) return;
            Log.Error("Disconnected from distributer");
        }
    }
}

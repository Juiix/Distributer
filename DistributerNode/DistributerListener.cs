using DistributerLib.Net;
using DistributerLib.Net.Packets;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Utils.NET.Net.Tcp;

namespace DistributerNode
{
    public interface IDistributerDelegate
    {

    }

    public class DistributerListener : NetConnectionFactory<DistributerConnection, DPacket>
    {

        private NetListener<DistributerConnection, DPacket> listener;

        public DistributerListener()
        {
            listener = new NetListener<DistributerConnection, DPacket>(12321, this);
        }

        public override DistributerConnection CreateConnection(Socket socket)
        {
            return new DistributerConnection(socket);
        }

        public override void HandleConnection(DistributerConnection connection)
        {

        }

        public void Start()
        {
            listener.Start();
        }

        public void Stop()
        {
            listener.Stop();
        }
    }
}

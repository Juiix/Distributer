using DistributerLib.Components;
using DistributerLib.Net;
using DistributerLib.Net.Packets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Utils.NET.Logging;
using Utils.NET.Net.Tcp;

namespace Distributer
{
    public class Distributer
    {
        private class Node
        {
            public int retryCount;
            public NodeConfig config;
            public DistributerConnection connection;

            public Node(NodeConfig config, DistributerConnection connection)
            {
                this.config = config;
                this.connection = connection;
            }
        }

        public static void Distribute(ConfigFile config)
        {
            var dist = new Distributer(config);
            dist.Run();
        }

        private ConfigFile config;
        private ConcurrentDictionary<NetConnection<DPacket>, Node> pendingConnections = new ConcurrentDictionary<NetConnection<DPacket>, Node>();
        private ConcurrentDictionary<NetConnection<DPacket>, Node> connections = new ConcurrentDictionary<NetConnection<DPacket>, Node>();

        private Dictionary<NodeConfig, string> packages = new Dictionary<NodeConfig, string>();

        public Distributer(ConfigFile config)
        {
            this.config = config;
        }

        public void Run()
        {
            Log.Write("Connecting to nodes...");

            CreateConnections();

            Log.Run();
        }

        #region Package Files



        #endregion

        #region Connection

        private void CreateConnections()
        {
            foreach (var nodeConfig in config.configs)
            {
                var connection = new DistributerConnection();
                pendingConnections.TryAdd(connection, new Node(nodeConfig, connection));
                connection.ConnectAsync(nodeConfig.host, DistributerConnection.Port, OnConnect);
            }
        }

        private void OnConnect(bool success, NetConnection<DPacket> net)
        {
            if (success)
            {
                if (pendingConnections.TryRemove(net, out var node))
                {
                    connections.TryAdd(net, node);
                    Log.Write($"Connected to {node.config.name}, {connections.Count} / {config.configs.Count} nodes connected", ConsoleColor.Green);
                }
                else
                {

                }
            }
            else
            {
                if (pendingConnections.TryGetValue(net, out var node))
                {
                    node.retryCount++;
                    if (node.retryCount < 5)
                    {
                        node.connection.ConnectAsync(node.config.host, DistributerConnection.Port, OnConnect);
                    }
                    else
                    {
                        Log.Error("Failed to connect to " + node.config.name);
                        pendingConnections.TryRemove(net, out node);
                        node.connection.Disconnect();
                    }
                }
            }
        }

        #endregion
    }
}

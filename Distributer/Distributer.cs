using DistributerLib.Components;
using DistributerLib.Net;
using DistributerLib.Net.Packets;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils.NET.Logging;
using Utils.NET.Net.Tcp;

namespace Distributer
{
    public class Distributer
    {
        private enum DistributionState
        {
            Packaging,
            Connecting,
            VersionCheck,
            Sending
        }

        private class Node
        {
            public int retryCount;
            public NodeConfig config;
            public DistributerConnection connection;
            public byte[] checksum;
            public bool needsUpdate = true;

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

        /// <summary>
        /// The configuration file containing node config information
        /// </summary>
        private ConfigFile config;

        /// <summary>
        /// Dictionary containing pending node connections
        /// </summary>
        private ConcurrentDictionary<NetConnection<DPacket>, Node> pendingConnections = new ConcurrentDictionary<NetConnection<DPacket>, Node>();

        /// <summary>
        /// Dictionary containing all connected nodes
        /// </summary>
        private ConcurrentDictionary<NetConnection<DPacket>, Node> connections = new ConcurrentDictionary<NetConnection<DPacket>, Node>();

        /// <summary>
        /// Dictionary of package file locations
        /// </summary>
        private Dictionary<NodeConfig, string> packages = new Dictionary<NodeConfig, string>();

        /// <summary>
        /// The current state of distribution
        /// </summary>
        private DistributionState state = DistributionState.Packaging;

        /// <summary>
        /// Count to be used by the state
        /// </summary>
        private int stateCount = 0;

        public Distributer(ConfigFile config)
        {
            this.config = config;
        }

        public void Run()
        {
            try
            {
                Start();

                Log.Run();
            }
            finally
            {
                CleanUpTemp();
            }
        }

        /// <summary>
        /// Ends the program execution
        /// </summary>
        private void EndProgram()
        {
            Log.Stop();
        }

        #region Program state

        private async void Start()
        {
            await Task.Run(() => RunState(state));
        }

        /// <summary>
        /// Trys to set the state if the state equals the fromState
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        private void SetState(DistributionState state, DistributionState fromState)
        {
            lock (this)
            {
                if (this.state != fromState) return;
                this.state = state;
                RunState(state);
            }
        }

        /// <summary>
        /// Start the given state's execution
        /// </summary>
        /// <param name="state"></param>
        private void RunState(DistributionState state)
        {
            switch (state)
            {
                case DistributionState.Packaging:
                    StartPackaging();
                    break;
                case DistributionState.Connecting:
                    StartConnecting();
                    return;
                case DistributionState.VersionCheck:
                    StartVersionCheck();
                    return;
                case DistributionState.Sending:
                    StartSending();
                    return;
            }
        }

        private void StartPackaging()
        {
            Log.Write("Compressing and packaging files...");

            if (!CreateNodePackages())
            {
                Log.Error("Failed to package nodes");
                EndProgram();
                return;
            }

            SetState(DistributionState.Connecting, DistributionState.Packaging);
        }

        private void StartConnecting()
        {
            Log.Write("Connecting to nodes...");

            CreateConnections();
        }

        private void StartVersionCheck()
        {
            Log.Write("Comparing versions...");

            CheckVersions();
        }

        private void StartSending()
        {
            Log.Write("Sending updates...");

            SendUpdates();
        }

        #endregion

        #region Package Files

        /// <summary>
        /// Compresses and packages files for each node
        /// </summary>
        private bool CreateNodePackages()
        {
            if (Directory.Exists(".temp"))
                Directory.Delete(".temp", true);
            var di = Directory.CreateDirectory(".temp");
            di.Attributes |= FileAttributes.Hidden;

            foreach (NodeConfig nodeConfig in config.configs)
            {
                if (!PackageNode(nodeConfig))
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Packages a node's directory
        /// </summary>
        /// <param name="nodeConfig"></param>
        private bool PackageNode(NodeConfig nodeConfig)
        {
            if (!Directory.Exists(nodeConfig.root))
            {
                Log.Error($"Directory \"{nodeConfig.root}\" does not exist for Node: {nodeConfig.name}");
                return false;
            }

            string name = $"{nodeConfig.name}-{packages.Count + 1}";
            string nodeTempDirectory = $".temp/{name}";
            string nodePackagePath = $".temp/{name}.zip";
            string rootZip = Path.Combine(nodeTempDirectory, $"{Path.GetFileName(nodeConfig.root)}.zip");
            string nodeConfigPath = Path.Combine(nodeTempDirectory, "config.json");

            Directory.CreateDirectory(nodeTempDirectory); // create a place to store temp files for this node
            ZipFile.CreateFromDirectory(nodeConfig.root, rootZip); // compress root directory into node temp directory
            File.WriteAllText(nodeConfigPath, JsonConvert.SerializeObject(nodeConfig, Formatting.Indented)); // write node config json into package
            ZipFile.CreateFromDirectory(nodeTempDirectory, nodePackagePath); // compress node package directory (node temp directory)

            packages[nodeConfig] = nodePackagePath;

            return true;
        }

        /// <summary>
        /// Deletes the temp directory
        /// </summary>
        private void CleanUpTemp()
        {
            if (Directory.Exists(".temp"))
                Directory.Delete(".temp", true);
        }

        #endregion

        #region Connection

        /// <summary>
        /// Creates and starts connections to all nodes
        /// </summary>
        private void CreateConnections()
        {
            foreach (var nodeConfig in config.configs)
            {
                var connection = new DistributerConnection();
                pendingConnections.TryAdd(connection, new Node(nodeConfig, connection));
                connection.ConnectAsync(nodeConfig.host, DistributerConnection.Port, OnConnect);
            }
        }

        /// <summary>
        /// Called when a node's connect function ends
        /// </summary>
        /// <param name="success"></param>
        /// <param name="net"></param>
        private void OnConnect(bool success, NetConnection<DPacket> net)
        {
            if (success)
            {
                if (pendingConnections.TryRemove(net, out var node))
                {
                    connections.TryAdd(net, node);
                    net.ReadAsync();
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
                        Log.Error("Failed to connect to node: " + node.config.name);
                        pendingConnections.TryRemove(net, out node);
                        node.connection.Disconnect();

                        if (pendingConnections.Count == 0) // end program
                        {
                            EndProgram();
                        }
                    }
                }
            }

            CheckConnectionFinished();
        }

        /// <summary>
        /// Checks if all nodes have finished connecting
        /// </summary>
        private void CheckConnectionFinished()
        {
            if (connections.Count != config.configs.Count) return;

            SetState(DistributionState.VersionCheck, DistributionState.Connecting);
        }

        #endregion

        #region Version Checking

        /// <summary>
        /// Checks node versions and determines the nodes that need an update
        /// </summary>
        public void CheckVersions()
        {
            foreach (var node in connections.Values)
            {
                var versionCheck = new VersionCheck();
                versionCheck.packageName = node.config.name;
                versionCheck.checksum = GenerateChecksum(packages[node.config]);
                node.checksum = versionCheck.checksum;
                node.connection.SendToken(versionCheck, ReceivedVersionResponse);
            }
        }

        private void ReceivedVersionResponse(DPacket packet, NetConnection<DPacket> net)
        {
            if (!(packet is VersionCheckResponse response)) return;
            if (!connections.TryGetValue(net, out var node)) return;

            if (response.needsUpdate)
                Log.Write($"{node.config.name} does need to update", ConsoleColor.Green);
            else
                Log.Write($"{node.config.name} does NOT need to update", ConsoleColor.Cyan);

            node.needsUpdate = response.needsUpdate;
            var count = Interlocked.Increment(ref stateCount);
            if (count != connections.Count) return;
            SetState(DistributionState.Sending, DistributionState.VersionCheck);
        }

        /// <summary>
        /// Generates an MD5 checksum for the given file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private byte[] GenerateChecksum(string file)
        {
            byte[] hash;
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(file))
                {
                    hash = md5.ComputeHash(stream);
                }
            }
            return hash;
        }

        #endregion

        #region Sending

        /// <summary>
        /// Dispatches the updates to nodes
        /// </summary>
        private void SendUpdates()
        {
            var updatables = connections.Values.Where(_ => _.needsUpdate);
            stateCount = updatables.Count();
            foreach (var node in updatables)
            {
                node.retryCount = 0;
                SendUpdate(node);
            }
        }

        /// <summary>
        /// Sends the update file and checksum to the node
        /// </summary>
        /// <param name="node"></param>
        private void SendUpdate(Node node)
        {
            var update = new UpdateFile();
            update.checksum = node.checksum;
            update.file = File.ReadAllBytes(packages[node.config]);
            update.packageName = node.config.name;
            node.connection.SendToken(update, ReceivedUpdateResponse);
        }


        private void ReceivedUpdateResponse(DPacket packet, NetConnection<DPacket> net)
        {
            if (!(packet is UpdateFileResponse response)) return;
            if (!connections.TryGetValue(net, out var node)) return;

            if (!response.success)
            {
                if (node.retryCount++ < 5)
                {
                    Log.Write($"{node.config.name} failed to receive the update! Retrying...", ConsoleColor.Red);
                    SendUpdate(node);
                }
                else
                    Log.Write($"{node.config.name} failed to receive the update!", ConsoleColor.Red);
                return;
            }

            Log.Write($"{node.config.name} successfully received the update", ConsoleColor.Green);
            var count = Interlocked.Decrement(ref stateCount);
            if (count != 0) return;

            // advance state
        }

        #endregion
    }
}

using DistributerLib.Components;
using DistributerLib.Net;
using DistributerLib.Net.Packets;
using System;
using System.IO;
using Utils.NET.IO;
using Utils.NET.IO.Tbon;
using Utils.NET.Logging;

namespace Distributer
{
    class Program
    {
        private const string Argument_Help = "distribute {config file path}";

        static void Main(string[] args)
        {
            var obj = Tbon.DynamicParse("config.tbon");

            Log.Write(((TObject)obj["configs"])[1]["name"].Value<string>());

            if (args.Length == 0)
            {
                Log.Error("Invalid arguments provided, correct usage:");
                Log.Error(Argument_Help);
                return;
            }

            string path = args[0];

            if (!path.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
            {
                Log.Error("Invalid config file.");
                Log.Error("Congif file must be of type .json");
                return;
            }

            if (!File.Exists(path))
            {
                Log.Error("Given config file cannot be found!");
                return;
            }

            ConfigFile config;
            try
            {
                config = ConfigFile.LoadFromFile(path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return;
            }

            Distributer.Distribute(config);
        }
    }
}

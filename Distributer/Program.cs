using DistributerLib.Components;
using System;
using System.IO;
using Utils.NET.IO;

namespace Distributer
{
    class Program
    {
        private const string Argument_Help = "distribute {config file path}";

        static void Main(string[] args)
        {
            BitWriter w = new BitWriter();
            w.Write(true);

            w.Write((byte)123);
            w.Write((ushort)12345);
            w.Write((uint)1234567);
            w.Write((ulong)12345678987654321);

            w.Write((sbyte)123);
            w.Write((short)12345);
            w.Write((int)1234567);
            w.Write((long)12345678987654321);

            w.Write(123.456f);
            w.Write(123.456);

            var d = w.GetData();
            BitReader r = new BitReader(d);
            Console.WriteLine(r.ReadBool());

            Console.WriteLine(r.ReadUInt8());
            Console.WriteLine(r.ReadUInt16());
            Console.WriteLine(r.ReadUInt32());
            Console.WriteLine(r.ReadUInt64());

            Console.WriteLine(r.ReadInt8());
            Console.WriteLine(r.ReadInt16());
            Console.WriteLine(r.ReadInt32());
            Console.WriteLine(r.ReadInt64());

            Console.WriteLine(r.ReadFloat());
            Console.WriteLine(r.ReadDouble());

            if (args.Length == 0)
            {
                Console.WriteLine("Invalid arguments provided, correct usage:");
                Console.WriteLine(Argument_Help);
                return;
            }

            string path = args[0];

            if (!path.EndsWith(".json"))
            {
                Console.WriteLine("Invalid config file.");
                Console.WriteLine("Congif file must be of type .json");
                return;
            }

            if (!File.Exists(path))
            {
                Console.WriteLine("Given config file cannot be found!");
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

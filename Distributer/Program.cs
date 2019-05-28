using System;
using System.IO;

namespace Distributer
{
    class Program
    {
        private const string Argument_Help = "distribute {config file path}";

        static void Main(string[] args)
        {
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


        }
    }
}

using System;
using Utils.NET.Logging;

namespace DistributerNode
{
    class Program
    {
        static void Main(string[] args)
        {
            var listener = new DistributerListener();
            listener.Start();

            Log.Run();

            listener.Stop();
        }
    }
}

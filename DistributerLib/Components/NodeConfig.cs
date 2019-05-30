using System;
using System.Collections.Generic;
using System.Text;

namespace DistributerLib.Components
{
    public class NodeConfig
    {
        public string name;

        public string host;

        public string root;

        public string executable;

        public IList<string> dependencies;
    }
}

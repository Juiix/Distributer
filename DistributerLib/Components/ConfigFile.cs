using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace DistributerLib.Components
{
    public class ConfigFile
    {
        public static ConfigFile LoadFromFile(string file)
        {
            return JsonConvert.DeserializeObject<ConfigFile>(File.ReadAllText(file));
        }

        public IList<NodeConfig> configs;
    }
}

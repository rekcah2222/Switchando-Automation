/*using HomeAutomationCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.ConfigRetriver
{
    class ConfigManager
    {
        static void Load()
        {
            if (!File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/server-config.json"))
            {
                //init as new
                return;
            }
            dynamic config = JsonConvert.DeserializeObject<ExpandoObject>(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/server-config.json");
            
        }
        static void Save()
        {

        }
    }
}
*/
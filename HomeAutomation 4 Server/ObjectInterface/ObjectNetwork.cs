using HomeAutomationCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace HomeAutomation.ObjectInterfaces
{
    public class ObjectNetwork
    {
        public List<MethodInterface> MethodInterfaces { get; set; }
        public List<ObjectInterface> ObjectInterfaces { get; set; }
        public List<Action> Actions { get; set; }

        public void Load()
        {
            if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/configuration_objectnetwork.json"))
            {
                string json = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/configuration_objectnetwork.json");
                HomeAutomationServer.server.ObjectNetwork = JsonConvert.DeserializeObject<ObjectNetwork>(json);
            }
        }
    }
}

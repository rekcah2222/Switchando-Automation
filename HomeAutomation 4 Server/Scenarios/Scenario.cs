using HomeAutomation.Network;
using HomeAutomationCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Scenarios
{
    public class Scenario
    {
        public string Name;
        public List<String> Commands;

        public Scenario(string name)
        {
            this.Name = name;
            this.Commands = new List<String>();

            HomeAutomationServer.server.Scenarios.Add(this);
            SaveAll();
        }

        class Deserialize
        {
            public string Name;
            public string[] Commands;

            public Scenario ConvertIntoScenario()
            {
                Console.WriteLine(Name);
                Console.WriteLine(Commands);
                Scenario scenario = new Scenario(Name);
                foreach(string command in Commands)
                {
                    scenario.Commands.Add(command);
                }
                SaveAll();
                return scenario;
            }
        }

        public static void Load()
        {
            if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "scenarios.json"))
            {
                string json = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "scenarios.json");
                Console.WriteLine("Scenarios data -> " + json);
                if (string.IsNullOrEmpty(json))
                {
                    HomeAutomationServer.server.Scenarios = new List<Scenario>();
                    return;
                }
                Deserialize[] scnrs = JsonConvert.DeserializeObject<Deserialize[]>(json);
                HomeAutomationServer.server.Scenarios = new List<Scenario>();
                foreach (Deserialize scnr in scnrs)
                {
                    scnr.ConvertIntoScenario();
                }
                //HomeAutomationServer.server.Scenarios = scnrs;
            }
            else
            {
                HomeAutomationServer.server.Scenarios = new List<Scenario>();
            }

            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("scenario")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("scenario", requestHandler);
        }

        public static void SaveAll()
        {
            string json = JsonConvert.SerializeObject(HomeAutomationServer.server.Scenarios);
            File.WriteAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "scenarios.json", json);
        }

        public void AddCommand(string command)
        {
            this.Commands.Add(command);
            SaveAll();
        }

        [System.Obsolete("RemoveCommand is deprecated, please use RemoveCommandContains instead.")]
        public void RemoveCommand(string command)
        {
            this.Commands.Remove(command);
            SaveAll();
        }

        public void RemoveCommandContains(string command)
        {
            foreach(string cmd in this.Commands)
            {
                if (cmd.Contains(command))
                {
                    this.Commands.Remove(cmd);
                    return;
                }
            }
            SaveAll();
        }

        public static void RemoveScenario(string name)
        {
            Scenario scenario = null;
            foreach (Scenario obj in HomeAutomationServer.server.Scenarios)
            {
                if (obj.GetName().ToLower().Equals(name.ToLower()))
                {
                    scenario = obj;
                    break;
                }
            }
            HomeAutomationServer.server.Scenarios.Remove(scenario);
            SaveAll();
        }

        public void Execute()
        {
            foreach(string message in Commands)
            {
                string[] commands = message.Split('&');

                string[] icommand = commands[0].Split('=');
                if (icommand[0].Equals("interface"))
                {
                    foreach (NetworkInterface networkInterface in HomeAutomationServer.server.NetworkInterfaces)
                    {
                        if (networkInterface.Id.Equals(icommand[1]))
                        {
                            Console.WriteLine("EXECUTED -> " + commands[1]);
                            HomeAutomationServer.server.Telegram.Log("Executing `" + commands[1] + "` scenario.");
                            networkInterface.Run(commands);
                        }
                    }
                }
            }
        }

        public static void SendParameters(string[] request)
        {
            Scenario scenario = null;

            foreach (string cmd in request)
            {
                string[] command = cmd.Split('=');
                if (command[0].Equals("interface")) continue;
                switch (command[0])
                {
                    case "objname":
                        foreach (Scenario obj in HomeAutomationServer.server.Scenarios)
                        {
                            if (obj.GetName().ToLower().Equals(command[1].ToLower()))
                            {
                                scenario = obj;
                                break;
                            }
                        }
                        break;

                    case "newScenario":
                        new Scenario(command[1]);
                        break;

                    case "addCommand":
                        scenario.AddCommand(command[1].Replace("%", "&").Replace("<", "="));
                        break;

                    case "removeCommand":
                        scenario.RemoveCommandContains(command[1]);
                        break;

                    case "execute":
                        scenario.Execute();
                        break;

                    case "removeScenario":
                        RemoveScenario(command[1]);
                        break;
                }
            }
        }

        public string GetName()
        {
            return this.Name;
        }
    }
}
using HomeAutomation.Network;
using HomeAutomation.Network.APIStatus;
using HomeAutomation.ObjectInterfaces;
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
        public string Description;
        public List<string> Actions;

        public Scenario(string name, string description)
        {
            this.Name = name;
            this.Description = description;
            HomeAutomationServer.server.Scenarios.Add(this);
            SaveAll();

            NetworkInterface.Delegate requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("scenario", requestHandler);
        }

        public static void SaveAll()
        {
            //TODO
        }

        public void AddAction(string action)
        {
            this.Actions.Add(action);
            SaveAll();
        }

        public void RemoveAction(string action)
        {
            this.Actions.Remove(action);
            SaveAll();
        }

        public static void RemoveScenario(string name)
        {
            Scenario scenario = null;
            foreach (Scenario obj in HomeAutomationServer.server.Scenarios)
            {
                if (obj.Name.ToLower().Equals(name.ToLower()))
                {
                    scenario = obj;
                    break;
                }
            }
            HomeAutomationServer.server.Scenarios.Remove(scenario);
            SaveAll();
        }

        public void Run()
        {
            foreach (string action in Actions)
            {
                ObjectInterfaces.Action.FromName(action).Run();
            }
        }

        public static string SendParameters(string method, string[] request)
        {
            if (method.Equals("createScenario"))
            {
                string name = null;
                string description = null;
                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            name = command[1];
                            break;
                        case "description":
                            description = command[1];
                            break;
                    }
                }
                if (name == null) return new ReturnStatus(CommonStatus.ERROR_BAD_REQUEST).Json();
                new Scenario(name, description);
                ReturnStatus data = new ReturnStatus(CommonStatus.SUCCESS);
                return data.Json();
            }
            if (method.Equals("addAction"))
            {
                Scenario scenario = null;

                string name = null;
                string action = null;
                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            name = command[1];
                            break;
                        case "action":
                            action = command[1];
                            break;
                    }
                }
                foreach (Scenario obj in HomeAutomationServer.server.Scenarios)
                {
                    if (obj.Name.ToLower().Equals(name.ToLower()))
                    {
                        scenario = obj;
                        break;
                    }
                }
                if (scenario == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND, name + " not found").Json();

                scenario.AddAction(action);

                ReturnStatus data = new ReturnStatus(CommonStatus.SUCCESS);
                return data.Json();
            }
            if (method.Equals("removeAction"))
            {
                Scenario scenario = null;

                string name = null;
                string action = null;
                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            name = command[1];
                            break;
                        case "action":
                            action = command[1];
                            break;
                    }
                }
                foreach (Scenario obj in HomeAutomationServer.server.Scenarios)
                {
                    if (obj.Name.ToLower().Equals(name.ToLower()))
                    {
                        scenario = obj;
                        break;
                    }
                }
                if (scenario == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND, name + " not found").Json();

                scenario.RemoveAction(action);

                ReturnStatus data = new ReturnStatus(CommonStatus.SUCCESS);
                return data.Json();
            }
            if (method.Equals("run") || method.Equals("execute"))
            {
                Scenario scenario = null;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            foreach (Scenario obj in HomeAutomationServer.server.Scenarios)
                            {
                                if (obj.Name.ToLower().Equals(command[1].ToLower()))
                                {
                                    scenario = obj;
                                    break;
                                }
                            }
                            break;
                    }
                }
                if (scenario == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND, "Scenario not found").Json();

                scenario.Run();

                ReturnStatus data = new ReturnStatus(CommonStatus.SUCCESS);
                return data.Json();
            }
            return "";
        }
    }
}
using HomeAutomation.Network;
using HomeAutomation.Network.APIStatus;
using HomeAutomation.Objects;
using HomeAutomationCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.ObjectInterfaces
{
    public class Action
    {
        public string Name;
        public string Description;
        public MethodInterface Method;
        public Dictionary<string, object> Parameters;
        public Condition[] Conditions;

        public Action(string name, string description, MethodInterface methodInterface, Dictionary<string, object> parameters, Condition[] conditions)
        {
            this.Name = name;
            this.Description = description;
            this.Method = methodInterface;
            this.Parameters = parameters;
            this.Conditions = conditions;
            HomeAutomationServer.server.ObjectNetwork.Actions.Add(this);
        }
        public string Run()
        {
            foreach(Condition condition in Conditions)
            {
                if (!condition.Verify())
                {
                    return new ReturnStatus(CommonStatus.SUCCESS, "Action didn't run because of some unverified conditions").Json();
                }
            }
            return Method.Run(Parameters);
        }
        public static Action FromName(string name)
        {
            foreach(Action action in HomeAutomationServer.server.ObjectNetwork.Actions)
            {
                if (action.Name.Equals(name)) return action;
            }
            return null;
        }
        public static string SendParameters(string method, string[] request)
        {
            if (method.Equals("createAction"))
            {
                /*string[] dataRow = method.Substring("createAction/".Length).Split('/');
                if (dataRow == null || dataRow[0] == null) return new ReturnStatus(CommonStatus.ERROR_BAD_REQUEST).Json();

                string name = dataRow[0];*/
                string name = null;
                string description = null;

                string methodInterface = null;
                string methodName = null;
                string jsonParameters = null;
                string jsonConditions = null;
                MethodInterface actionMethod = null;

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
                        case "interface":
                            methodInterface = command[1];
                            break;
                        case "method":
                            methodName = command[1];
                            break;
                        case "parameters":
                            jsonParameters = command[1];
                            break;
                        case "conditions":
                            jsonConditions = command[1];
                            break;
                    }
                }
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(description)) return new ReturnStatus(CommonStatus.ERROR_BAD_REQUEST).Json();

                actionMethod = MethodInterface.FromString(methodInterface, method);
                if (actionMethod == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND, "Method not found").Json();

                Dictionary<string, object> parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonParameters);

                Condition[] conditions = new Condition[0];
                if (!string.IsNullOrEmpty(jsonConditions))
                {
                    conditions = JsonConvert.DeserializeObject<Condition[]>(jsonConditions);
                }

                Action action = new Action(name, description, actionMethod, parameters, conditions);

                ReturnStatus data = new ReturnStatus(CommonStatus.SUCCESS);
                data.Object.action = action;
                return data.Json();
            }
            if (method.Equals("getAction"))
            {
                Action action = null;
                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            action = Action.FromName(command[1]);
                            break;
                    }
                }
                if (action == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND).Json();

                ReturnStatus data = new ReturnStatus(CommonStatus.SUCCESS);
                data.Object.action = action;
                return data.Json();
            }

            if (method.StartsWith("runAction/name"))
            {
                Action action = null;
                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            action = Action.FromName(command[1]);
                            break;
                    }
                }
                if (action == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND).Json();

                return action.Run();
            }

            return new ReturnStatus(CommonStatus.ERROR_NOT_IMPLEMENTED, "Not implemented").Json();
        }
    }
}

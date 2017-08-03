using HomeAutomation.Network;
using HomeAutomation.Objects.Inputs;
using HomeAutomation.Objects.Switches;
using HomeAutomationCore;
using HomeAutomationCore.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Objects.External
{
    class WebRelay : ISwitch
    {
        public string Name;
        public string ID;
        public string[] FriendlyNames;
        public bool Enabled;
        public string Description;
        public bool Online = false;
        public string Address;

        public HomeAutomationObject ObjectType = HomeAutomationObject.EXTERNAL_SWITCH;

        public WebRelay()
        {
            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("webrelay")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("webrelay", requestHandler);
        }
        public WebRelay(string name, string id, string description, string[] friendlyNames)
        {
            this.FriendlyNames = friendlyNames;
            this.Description = description;
            this.Name = name;
            this.ID = id;
            HomeAutomationServer.server.Objects.Add(this);

            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("webrelay")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("webrelay", requestHandler);
        }

        public void Start()
        {
            Console.WriteLine("Switch `" + this.Name + "` has been turned on.");
            //HomeAutomationServer.server.Telegram.Log("Switch `" + this.Name + "` has been turned on.");

            new WebClient().UploadString("http://" + Address + "/HomeAutomation/" + ID + "/on", "");

            Enabled = true;
        }
        public void Stop()
        {
            Console.WriteLine("Switch `" + this.Name + "` has been turned off.");
            //HomeAutomationServer.server.Telegram.Log("Switch `" + this.Name + "` has been turned off.");

            new WebClient().UploadString("http://" + Address + "/HomeAutomation/" + ID + "/off", "");

            Enabled = false;
        }
        public void AddButton()
        {
            Client client = null;
            foreach (Client clnt in HomeAutomationServer.server.Clients)
            {
                if (client.Name.Equals("local")) client = clnt;
            }
            new Button(client, ID + "_btn", true);
        }
        public void AddSwitchButton()
        {
            Client client = null;
            foreach (Client clnt in HomeAutomationServer.server.Clients)
            {
                if (client.Name.Equals("local")) client = clnt;
            }
            new SwitchButton(client, ID + "_swbtn", true);
        }
        public bool IsOn()
        {
            return Enabled;
        }
        public string GetName()
        {
            return Name;
        }
        public string GetId()
        {
            return ID;
        }
        public HomeAutomationObject GetObjectType()
        {
            return HomeAutomationObject.EXTERNAL_SWITCH;
        }
        public string[] GetFriendlyNames()
        {
            return FriendlyNames;
        }
        public NetworkInterface GetInterface()
        {
            return NetworkInterface.FromId("webrelay");
        }
        public static void SendParameters(string[] request)
        {
            WebRelay webrelay = null;
            foreach (string cmd in request)
            {
                string[] command = cmd.Split('=');
                if (command[0].Equals("interface")) continue;
                switch (command[0])
                {
                    case "online":
                        string[] idip = command[1].Split('@');
                        foreach (IObject obj in HomeAutomationServer.server.Objects)
                        {
                            if (obj.GetObjectType() == HomeAutomationObject.EXTERNAL_SWITCH)
                            {
                                WebRelay relay = (WebRelay)obj;
                                if (relay.ID.Equals(idip[0]))
                                {
                                    relay.Address = idip[1];
                                    relay.Online = true;
                                }
                            }
                        }
                        break;

                    case "objname":
                        foreach (IObject obj in HomeAutomationServer.server.Objects)
                        {
                            if (obj.GetName().Equals(command[1]))
                            {
                                webrelay = (WebRelay)obj;
                            }
                            if (Array.IndexOf(obj.GetFriendlyNames(), command[1].ToLower()) > -1)
                            {
                                webrelay = (WebRelay)obj;
                            }
                        }
                        break;

                    case "switch":
                        if (command[1].ToLower().Equals("true"))
                        {
                            webrelay.Start();
                        }
                        else
                        {
                            webrelay.Stop();
                        }
                        break;
                    case "addButton":
                        if (command[1].ToLower().Equals("button"))
                        {
                            webrelay.AddButton();
                        }
                        else if (command[1].ToLower().Equals("switch_button"))
                        {
                            webrelay.AddSwitchButton();
                        }
                        break;
                }
            }
        }
    }
}
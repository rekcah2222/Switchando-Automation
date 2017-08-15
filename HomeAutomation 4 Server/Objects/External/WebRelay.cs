using HomeAutomation.Network;
using HomeAutomation.Objects.Inputs;
using HomeAutomation.Objects.Switches;
using HomeAutomation.Rooms;
using HomeAutomationCore;
using HomeAutomationCore.Client;
using System;
using System.Net;

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
            if (Online)
            {
                try
                {
                    new WebClient().UploadString("http://" + Address + "/HomeAutomation/" + ID + "/on", "");
                }
                catch
                {
                    Online = false;
                }
            }

            Enabled = true;
        }
        public void Stop()
        {
            Console.WriteLine("Switch `" + this.Name + "` has been turned off.");
            //HomeAutomationServer.server.Telegram.Log("Switch `" + this.Name + "` has been turned off.");

            if (Online)
            {
                try
                {
                    new WebClient().UploadString("http://" + Address + "/HomeAutomation/" + ID + "/off", "");
                }
                catch
                {
                    Online = false;
                }
            }

            Enabled = false;
        }
        public void AddButton(Room room)
        {
            Client client = null;
            foreach (Client clnt in HomeAutomationServer.server.Clients)
            {
                if (clnt.Name.Equals("local")) client = clnt;
            }
            Button btn = new Button(client, ID + "_btn", true);
            btn.AddObject(this);
            room.AddItem(btn);
        }
        public void AddSwitchButton(Room room)
        {
            Client client = null;
            foreach (Client clnt in HomeAutomationServer.server.Clients)
            {
                if (client.Name.Equals("local")) client = clnt;
            }
            SwitchButton btn = new SwitchButton(client, ID + "_swbtn", true);
            btn.AddObject(this);
            room.AddItem(btn);
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
                            if (obj.GetFriendlyNames() == null) continue;
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
                }
            }
        }
    }
}
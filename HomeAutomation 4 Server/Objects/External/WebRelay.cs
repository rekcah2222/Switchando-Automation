using HomeAutomation.Network;
using HomeAutomation.Network.APIStatus;
using HomeAutomation.Objects.Inputs;
using HomeAutomation.Objects.Switches;
using HomeAutomation.Rooms;
using HomeAutomationCore;
using HomeAutomationCore.Client;
using System;
using System.Collections.Generic;
using System.Net;

namespace HomeAutomation.Objects.External
{
    public class WebRelay : ISwitch
    {
        public string Name;
        public string ID;
        public string[] FriendlyNames;
        public bool Switch;
        public string Description;
        public bool Online = false;
        public string Address;
        public string ClientName = "local";

        public string ObjectType = "EXTERNAL_SWITCH";
        public string ObjectModel = "SWITCH";

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
                    Console.WriteLine("Forwarding request...");
                    new WebClient().UploadString("http://" + Address + "/HomeAutomation/" + ID + "/on", "");
                    Console.WriteLine("http://" + Address + "/HomeAutomation/" + ID + "/on");
                }
                catch
                {
                    Online = false;
                }
            }

            Switch = true;
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

            Switch = false;
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
            return Switch;
        }
        public string GetName()
        {
            return Name;
        }
        public string GetId()
        {
            return ID;
        }
        public string GetObjectType()
        {
            return "EXTERNAL_SWITCH";
        }
        public string[] GetFriendlyNames()
        {
            return FriendlyNames;
        }
        public NetworkInterface GetInterface()
        {
            return NetworkInterface.FromId("webrelay");
        }
        private static WebRelay FindWebRelayFromName(string name)
        {
            WebRelay relay = null;
            foreach (IObject obj in HomeAutomationServer.server.Objects)
            {
                if (obj.GetName().ToLower().Equals(name.ToLower()))
                {
                    relay = (WebRelay)obj;
                    break;
                }
                if (obj.GetFriendlyNames() == null) continue;
                if (Array.IndexOf(obj.GetFriendlyNames(), name.ToLower()) > -1)
                {
                    relay = (WebRelay)obj;
                    break;
                }
            }
            return relay;
        }
        public static string SendParameters(string method, string[] request)
        {
            if (method.Equals("handshake"))
            {
                WebRelay webrelay = null;
                string address = null;
                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            foreach (IObject obj in HomeAutomationServer.server.Objects)
                            {
                                if (obj.GetObjectType() == "EXTERNAL_SWITCH")
                                {
                                    WebRelay myobj = (WebRelay)obj;
                                    if (myobj.ID.Equals(command[1]))
                                    {
                                        webrelay = myobj;
                                    }
                                }
                            }
                            break;
                        case "address":
                            address = command[1];
                            break;
                    }
                }
                if (webrelay == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND, "Id not found").Json();
                if (address == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND, "Address is null").Json();

                webrelay.Address = address;
                webrelay.Online = true;
                return new ReturnStatus(CommonStatus.SUCCESS).Json();
            }

            if (method.Equals("switch"))
            {
                WebRelay relay = null;
                bool status = false;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            relay = FindWebRelayFromName(command[1]);
                            break;
                        case "switch":
                            status = bool.Parse(command[1]);
                            break;
                    }
                    if (relay == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND).Json();
                }
                if (status) relay.Start(); else relay.Stop();
                return new ReturnStatus(CommonStatus.SUCCESS).Json();
            }

            if (string.IsNullOrEmpty(method))
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
                                if (obj.GetObjectType() == "EXTERNAL_SWITCH")
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
            return "";
        }
        public static void Setup(Room room, dynamic device)
        {
            WebRelay relay = new WebRelay();
            relay.Name = device.Name;
            relay.Description = device.Description;
            relay.Switch = device.Switch;
            relay.FriendlyNames = Array.ConvertAll(((List<object>)device.FriendlyNames).ToArray(), x => x.ToString());
            relay.Online = device.Online;
            relay.ID = device.ID;

            HomeAutomationServer.server.Objects.Add(relay);
            room.AddItem(relay);
        }
    }
}
using Homeautomation.GPIO;
using HomeAutomation.Network;
using HomeAutomation.Objects.Switches;
using HomeAutomationCore;
using HomeAutomationCore.Client;
using System;

namespace HomeAutomation.Objects.Fans
{
    class Relay : ISwitch
    {
        Client Client;
        public string ClientName;
        public uint Pin;
        public string Name;
        public string[] FriendlyNames;
        public bool Enabled;
        public string Description;

        public HomeAutomationObject ObjectType = HomeAutomationObject.GENERIC_SWITCH;

        public Relay()
        {
            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("relay")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("relay", requestHandler);
        }
        public Relay(Client client, string name, uint pin, string description, string[] friendlyNames)
        {
            this.Client = client;
            this.ClientName = client.Name;
            this.FriendlyNames = friendlyNames;

            this.Description = description;
            this.Pin = pin;
            this.Name = name;
            HomeAutomationServer.server.Objects.Add(this);

            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("relay")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("relay", requestHandler);
        }
        public void SetClient(Client client)
        {
            this.Client = client;
        }

        public void Start()
        {
            Console.WriteLine("Switch `" + this.Name + "` has been turned on.");
            HomeAutomationServer.server.Telegram.Log("Switch `" + this.Name + "` has been turned on.");
            if (Client.Name.Equals("local"))
            {
                PIGPIO.gpio_write(0, Pin, 1);
            }
            else
            {
                UploadValues(true);
            }
            Enabled = true;
        }
        public void Stop()
        {
            Console.WriteLine("Switch `" + this.Name + "` has been turned off.");
            HomeAutomationServer.server.Telegram.Log("Switch `" + this.Name + "` has been turned off.");
            if (Client.Name.Equals("local"))
            {
                PIGPIO.gpio_write(0, Pin, 0);
            }
            else
            {
                UploadValues(false);
            }
            Enabled = false;
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
            return Name;
        }
        public HomeAutomationObject GetObjectType()
        {
            return HomeAutomationObject.GENERIC_SWITCH;
        }
        public string[] GetFriendlyNames()
        {
            return FriendlyNames;
        }
        void UploadValues(bool Value)
        {
            Client.Sendata("interface=relay&objname=" + this.Name + "&enabled=" + Value.ToString());
        }
        public NetworkInterface GetInterface()
        {
            return NetworkInterface.FromId("relay");
        }
        public static void SendParameters(string[] request)
        {
            Relay fan = null;
            foreach (string cmd in request)
            {
                string[] command = cmd.Split('=');
                if (command[0].Equals("interface")) continue;
                switch (command[0])
                {
                    case "objname":
                        foreach (IObject obj in HomeAutomationServer.server.Objects)
                        {
                            if (obj.GetName().Equals(command[1]))
                            {
                                fan = (Relay)obj;
                            }
                            if (Array.IndexOf(obj.GetFriendlyNames(), command[1].ToLower()) > -1)
                            {
                                fan = (Relay)obj;
                            }
                        }
                        break;

                    case "switch":
                        if (command[1].ToLower().Equals("true"))
                        {
                            fan.Start();
                        }
                        else
                        {
                            fan.Stop();
                        }
                        break;
                }
            }
        }
    }
}
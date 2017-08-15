using Homeautomation.GPIO;
using HomeAutomation.Network;
using HomeAutomation.Objects.Switches;
using HomeAutomationCore;
using HomeAutomationCore.Client;
using System;
using System.Collections.Generic;
using System.Timers;

namespace HomeAutomation.Objects.Inputs
{
    class SwitchButton : IObject
    {
        Client Client;
        public string ClientName;

        public string Name;

        public uint Pin;
        bool Status;
        bool IsRemote;

        Timer timer;

        public List<string> CommandsOn;
        public List<string> CommandsOff;

        public List<string> Objects;

        public HomeAutomationObject ObjectType = HomeAutomationObject.SWITCH_BUTTON;
        public SwitchButton(Client client, string name, bool isRemote)
        {
            new SwitchButton(client, name, 0, IsRemote);
        }
        public SwitchButton(Client client, string name, uint pin, bool isRemote)
        {
            this.Client = client;
            this.ClientName = client.Name;
            this.Pin = pin;
            this.Name = name;
            this.IsRemote = isRemote;

            this.CommandsOn = new List<string>();
            this.CommandsOff = new List<string>();
            this.Objects = new List<string>();

            if (!isRemote)
            {
                if (Client.Name.Equals("local"))
                {
                    PIGPIO.set_pull_up_down(0, this.Pin, 2);
                    Console.WriteLine("PUD-UP was set on GPIO" + this.Pin);

                    timer = new Timer();

                    timer.Elapsed += Tick;
                    timer.Interval = 200;
                    timer.Start();
                }
            }

            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("switch_button")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("switch_button", requestHandler);

            HomeAutomationServer.server.Objects.Add(this);
        }
        public void SetClient(Client client)
        {
            this.Client = client;
        }

        public void AddCommand(string command, bool onoff)
        {
            if (onoff)
            {
                CommandsOn.Add(command.Replace("=", ",,").Replace("&", ",,,"));
            }
            else
            {
                CommandsOff.Add(command.Replace("=", ",,").Replace("&", ",,,"));
            }
        }
        public void RemoveCommand(string command, bool onoff)
        {
            if (onoff)
            {
                CommandsOn.Remove(command.Replace("=", ",,").Replace("&", ",,,"));
            }
            else
            {
                CommandsOff.Remove(command.Replace("=", ",,").Replace("&", ",,,"));
            }
        }
        public void RemoveObject(ISwitch obj)
        {
            Objects.Remove(obj.GetName());
        }
        public void AddObject(ISwitch obj)
        {
            Objects.Add(obj.GetName());
        }
        public void AddObject(string obj)
        {
            if (!Objects.Contains(obj))
                Objects.Add(obj);
        }
        public void RemoveObject(string obj)
        {
            if (Objects.Contains(obj))
                Objects.Remove(obj);
        }

        private void Tick(object sender, ElapsedEventArgs args)
        {
            int currentStatus = PIGPIO.gpio_read(0, Pin);
            bool lStatus;
            if (currentStatus == 1) lStatus = false; else lStatus = true;

            if (lStatus == this.Status) return;
            else
            {
                this.Status = lStatus;
                StatusChanged(lStatus);
            }
        }
        public void StatusChanged(bool value)
        {
            Console.WriteLine(this.Name + " status: " + this.Status);
            HomeAutomationServer.server.Telegram.Log("Button `" + this.Name + "`'s status: " + this.Status + ".");
            if (value)
            {
                foreach (string command in CommandsOn)
                {
                    var message = command.Replace(",,,", "&");
                    message = message.Replace(",,", "=");
                    string[] commands = message.Split('&');

                    string[] icommand = commands[0].Split('=');
                    if (icommand[0].Equals("interface"))
                    {
                        foreach (NetworkInterface networkInterface in HomeAutomationServer.server.NetworkInterfaces)
                        {
                            if (networkInterface.Id.Equals(icommand[1]))
                            {
                                networkInterface.Run(commands);
                            }
                        }
                    }
                }
                List<ISwitch> objectsList = new List<ISwitch>();
                foreach (IObject iobj in HomeAutomationServer.server.Objects)
                {
                    if (this.Objects.Contains(iobj.GetName()))
                    {
                        objectsList.Add((ISwitch)iobj);
                    }
                }
                foreach (ISwitch iobj in objectsList)
                {
                    iobj.Start();
                }
            }
            else
            {
                foreach (string command in CommandsOff)
                {
                    var message = command.Replace(",,,", "&");
                    message = message.Replace(",,", "=");
                    string[] commands = message.Split('&');

                    string[] icommand = commands[0].Split('=');
                    if (icommand[0].Equals("interface"))
                    {
                        foreach (NetworkInterface networkInterface in HomeAutomationServer.server.NetworkInterfaces)
                        {
                            if (networkInterface.Id.Equals(icommand[1]))
                            {
                                networkInterface.Run(commands);
                            }
                        }
                    }
                }
                List<ISwitch> objectsList = new List<ISwitch>();
                foreach (IObject iobj in HomeAutomationServer.server.Objects)
                {
                    if (this.Objects.Contains(iobj.GetName()))
                    {
                        objectsList.Add((ISwitch)iobj);
                    }
                }
                foreach (ISwitch iobj in objectsList)
                {
                    iobj.Stop();
                }
            }
        }
        public string GetName()
        {
            return this.Name;
        }
        public HomeAutomationObject GetObjectType()
        {
            return HomeAutomationObject.SWITCH_BUTTON;
        }
        public NetworkInterface GetInterface()
        {
            return null;
        }
        public string[] GetFriendlyNames()
        {
            return new string[0];
        }
        public static void SendParameters(string[] request)
        {
            SwitchButton button = null;
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
                                button = (SwitchButton)obj;
                            }
                        }
                        break;

                    case "switch":
                        if (command[1].ToLower().Equals("true"))
                        {
                            button.StatusChanged(true);
                        }
                        else
                        {
                            button.StatusChanged(false);
                        }
                        break;
                }
            }
        }
    }
}

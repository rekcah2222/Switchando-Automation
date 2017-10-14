using HomeAutomation.Network;
using HomeAutomation.Network.APIStatus;
using HomeAutomation.Objects.External;
using HomeAutomation.Objects.Fans;
using HomeAutomation.Objects.Switches;
using HomeAutomation.Rooms;
using HomeAutomationCore;
using HomeAutomationCore.Client;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HomeAutomation.Objects.Blinds
{
    class Blinds : ISwitch
    {
        Client Client;
        public string ClientName;

        public string Name;
        public string Description;
        public string[] FriendlyNames;

        public ISwitch OpenDevice;
        public ISwitch CloseDevice;

        public string ObjectType = "BLINDS";
        public string ObjectModel = "BLINDS";

        public int TotalSteps;
        public int Step;
        Thread movingThread;
        bool isMoving;

        public Blinds()
        {
            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("blinds")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("blinds", requestHandler);
        }
        public Blinds(Client client, string name, ISwitch openDevice, ISwitch closeDevice, int totalSteps, string description, string[] friendlyNames)
        {
            this.Client = client;
            this.Name = name;
            this.OpenDevice = openDevice;
            this.CloseDevice = closeDevice;
            this.Description = description;
            this.FriendlyNames = friendlyNames;
            this.TotalSteps = totalSteps;
            this.ClientName = client.Name;

            HomeAutomationServer.server.Objects.Add(this);

            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("blinds")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("blinds", requestHandler);
        }

        public void Start()
        {
            if (!Client.Name.Equals("local"))
            {
                Client.Sendata("interface=blinds&objname=" + this.Name + "&switch=true");
                return;
            }
            if (isMoving)
            {
                isMoving = false;
                return;
            }

            movingThread = new Thread(MovingThread);
            isMoving = true;
            movingThread.Start(0);
        }
        public void Stop()
        {
            if (!Client.Name.Equals("local"))
            {
                Client.Sendata("interface=blinds&objname=" + this.Name + "&switch=false");
                return;
            }
            if (isMoving)
            {
                isMoving = false;
                return;
            }

            movingThread = new Thread(MovingThread);
            isMoving = true;
            movingThread.Start(TotalSteps);
        }
        public void Move(int step)
        {
            //Console.WriteLine(step);
            if (!Client.Name.Equals("local"))
            {
                double d = (double)step / (double)TotalSteps;
                Console.WriteLine(d);
                int percentage = (int)Math.Round(d * 100d);
                Console.WriteLine(percentage);
                Client.Sendata("interface=blinds&objname=" + this.Name + "&percentage=" + percentage);
                return;
            }
            if (isMoving)
            {
                isMoving = false;
                Thread.Sleep(1500);
            }

            movingThread = new Thread(MovingThread);
            isMoving = true;
            movingThread.Start(step);
        }
        void MovingThread(object data)
        {
            int step = (int)data;

            if (step <= Step)
            {
                OpenDevice.Start();
                for (int pos = Step; pos >= step; pos--)
                {
                    Step = pos;
                    Thread.Sleep(1000);
                    if (isMoving == false)
                    {
                        OpenDevice.Stop();
                        return;
                    }
                }
                OpenDevice.Stop();
                isMoving = false;
            }
            else
            {
                CloseDevice.Start();
                for (int pos = Step; pos <= step; pos++)
                {
                    Step = pos;
                    Thread.Sleep(1000);
                    if (isMoving == false)
                    {
                        CloseDevice.Stop();
                        return;
                    }
                }
                CloseDevice.Stop();
                isMoving = false;
            }
        }
        public bool IsOn()
        {
            int percentage = Step / TotalSteps * 100;
            if (percentage <= 90) return false; else return true;
        }
        public string GetName()
        {
            return this.Name;
        }
        public string GetObjectType()
        {
            return "BLINDS";
        }
        public NetworkInterface GetInterface()
        {
            return NetworkInterface.FromId("blinds");
        }
        public string[] GetFriendlyNames()
        {
            return this.FriendlyNames;
        }
        private static Blinds FindBlindsFromName(string name)
        {
            Blinds blinds = null;
            foreach (IObject obj in HomeAutomationServer.server.Objects)
            {
                if (obj.GetName().ToLower().Equals(name.ToLower()))
                {
                    blinds = (Blinds)obj;
                    break;
                }
                if (obj.GetFriendlyNames() == null) continue;
                if (Array.IndexOf(obj.GetFriendlyNames(), name.ToLower()) > -1)
                {
                    blinds = (Blinds)obj;
                    break;
                }
            }
            return blinds;
        }
        static string SendParameters(string method, string[] request)
        {
            if (method.Equals("switch"))
            {
                Blinds blinds = null;
                bool status = false;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            blinds = FindBlindsFromName(command[1]);
                            break;
                        case "switch":
                            status = bool.Parse(command[1]);
                            break;
                    }
                }
                if (blinds == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND).Json();
                if (status) blinds.Start(); else blinds.Stop();
                return new ReturnStatus(CommonStatus.SUCCESS).Json();
            }

            if (method.Equals("move"))
            {
                Blinds blinds = null;
                int percentage = 255;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            blinds = FindBlindsFromName(command[1]);
                            break;
                        case "value":
                            percentage = int.Parse(command[1]);
                            break;
                    }
                }
                if (blinds == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND).Json();
                double prestep = (percentage / 100d) * blinds.TotalSteps;
                int step = (int)Math.Round(prestep);
                blinds.Move(step);

                return new ReturnStatus(CommonStatus.SUCCESS).Json();
            }

            if (method.Equals("internal/updateStep"))
            {
                Blinds blinds = null;
                int percentage = 255;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            blinds = FindBlindsFromName(command[1]);
                            break;
                        case "value":
                            percentage = int.Parse(command[1]);
                            break;
                    }
                }
                if (blinds == null) return new ReturnStatus(CommonStatus.ERROR_NOT_FOUND).Json();
                blinds.Step = percentage;

                return new ReturnStatus(CommonStatus.SUCCESS).Json();
            }

            if (string.IsNullOrEmpty(method))
            {
                Blinds blinds = null;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    if (command[0].Equals("interface")) continue;
                    switch (command[0])
                    {
                        case "objname":
                            foreach (IObject obj in HomeAutomationServer.server.Objects)
                            {
                                if (obj.GetName().ToLower().Equals(command[1].ToLower()))
                                {
                                    blinds = (Blinds)obj;
                                    break;
                                }
                                if (obj.GetFriendlyNames() == null) continue;
                                if (Array.IndexOf(obj.GetFriendlyNames(), command[1].ToLower()) > -1)
                                {
                                    blinds = (Blinds)obj;
                                    break;
                                }
                            }
                            break;
                        case "percentage":
                            int percentage = int.Parse(command[1]);
                            //Console.WriteLine(blinds.TotalSteps);
                            double prestep = (percentage / 100d) * blinds.TotalSteps;
                            int step = (int)Math.Round(prestep);
                            blinds.Move(step);
                            return "";
                        case "switch":
                            bool status = bool.Parse(command[1]);
                            if (status) blinds.Start(); else blinds.Stop();
                            return "";
                        case "update":
                            blinds.Step = int.Parse(command[1]);
                            return "";

                    }
                }
            }
            return "";
        }
        public static void Setup(Room room, dynamic device)
        {
            ISwitch openDevice;
            ISwitch closeDevice;
            if (device.OpenDevice.ObjectType == "EXTERNAL_SWITCH")
            {
                WebRelay relay = new WebRelay();
                relay.Name = device.OpenDevice.Name;
                relay.Description = device.OpenDevice.Description;
                relay.Switch = device.OpenDevice.Switch;
                relay.FriendlyNames = Array.ConvertAll(((List<object>)device.FriendlyNames).ToArray(), x => x.ToString());
                relay.Online = device.OpenDevice.Online;
                relay.ID = device.OpenDevice.ID;
                openDevice = relay;

                HomeAutomationServer.server.Objects.Add(relay);

                relay = new WebRelay();
                relay.Name = device.CloseDevice.Name;
                relay.Description = device.CloseDevice.Description;
                relay.Switch = device.CloseDevice.Switch;
                relay.FriendlyNames = Array.ConvertAll(((List<object>)device.FriendlyNames).ToArray(), x => x.ToString());
                relay.Online = device.CloseDevice.Online;
                relay.ID = device.CloseDevice.ID;
                closeDevice = relay;

                HomeAutomationServer.server.Objects.Add(relay);
            }
            else
            {
                Relay relay = new Relay();
                relay.Pin = (uint)device.OpenDevice.Pin;
                relay.Name = device.OpenDevice.Name;
                relay.Description = device.OpenDevice.Description;
                relay.FriendlyNames = Array.ConvertAll(((List<object>)device.FriendlyNames).ToArray(), x => x.ToString());
                relay.Switch = device.OpenDevice.Switch;
                relay.ClientName = device.Client.Name;
                relay.SetClient(device.Client);
                openDevice = relay;

                relay = new Relay();
                relay.Pin = (uint)device.CloseDevice.Pin;
                relay.Name = device.CloseDevice.Name;
                relay.Description = device.CloseDevice.Description;
                relay.FriendlyNames = Array.ConvertAll(((List<object>)device.FriendlyNames).ToArray(), x => x.ToString());
                relay.Switch = device.CloseDevice.Switch;
                relay.ClientName = device.Client.Name;
                relay.SetClient(device.Client);
                closeDevice = relay;
            }
            Client client = device.Client;
            Blinds blinds = new Blinds(client, device.Name, openDevice, closeDevice, (int)device.TotalSteps, device.Description, Array.ConvertAll(((List<object>)device.FriendlyNames).ToArray(), x => x.ToString()));

            room.AddItem(blinds);
        }
    }
}

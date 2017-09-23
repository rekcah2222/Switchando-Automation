using HomeAutomation.Dictionaries;
using HomeAutomation.Network;
using HomeAutomation.Objects;
using HomeAutomation.Objects.Lights;
using HomeAutomation.Objects.Switches;
using HomeAutomationCore;
using System;
using System.Collections.Generic;
using System.Threading;

namespace HomeAutomation.Rooms
{
    public class Room
    {
        public string Name;
        public string[] FriendlyNames;
        public List<IObject> Objects;
        public bool Hidden;
        public Room(string name, string[] friendlyNames, bool hidden)
        {
            this.Hidden = hidden;
            this.Name = name;
            this.FriendlyNames = friendlyNames;
            this.Objects = new List<IObject>();
            HomeAutomationServer.server.Rooms.Add(this);

            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("room")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("room", requestHandler);
        }
        public void AddItem(IObject homeAutomationObject)
        {
            this.Objects.Add(homeAutomationObject);
        }
        public void Switch(bool status)
        {
            foreach (IObject item in Objects)
            {
                //Console.WriteLine("switching_pre");
                if (item.GetObjectType().Equals("LIGHT_GPIO_RGB"))
                {
                    //Console.WriteLine("switching");
                    ((ILight)item).Pause(status);
                    Thread.Sleep(1000);
                 }
                else if (item.GetObjectType().Equals("GENERIC_SWITCH"))
                {
                    if (status) ((ISwitch)item).Start(); else ((ISwitch)item).Stop();
                }
            }
        }
        public void Color(uint R, uint G, uint B, int dimmer)
        {
            HomeAutomationServer.server.Telegram.Log("Changing color of room `" + this.Name + "`.");
            foreach (IObject item in Objects)
            {
                if (item.GetObjectType().Equals("LIGHT"))
                {
                    if (((ILight)item).GetLightType() == LightType.RGB_LIGHT)
                    {
                        ((IColorableLight)item).Set(R, G, B, dimmer);
                        Thread.Sleep(1000);
                    }
                    /*else if (item.GetType().Equals(typeof(RGBLight)))
                    {
                        ((RGBLight)item).Set(R, G, B, dimmer);
                    }*/
                }
            }
        }
        public void Dimm(uint percentace, int dimmer)
        {
            HomeAutomationServer.server.Telegram.Log("Dimming room `" + this.Name + "` to `" + percentace + "%`" + "(" + dimmer + "ms).");
            foreach (IObject item in Objects)
            {
                if (item.GetObjectType().Equals("LIGHT_GPIO_RGB"))
                {
                    ((ILight)item).Dimm(percentace, dimmer);
                    Thread.Sleep(1000);
                }
            }
        }
        public static string SendParameters(string[] request)
        {
            Room room = null;
            uint R = 0;
            uint G = 0;
            uint B = 0;
            int dimmer = 0;
            string color = null;
            uint dimm_percentage = 400;
            string status = null;

            foreach (string cmd in request)
            {
                string[] command = cmd.Split('=');
                if (command[0].Equals("interface")) continue;
                switch (command[0])
                {
                    case "objname":
                        foreach (Room obj in HomeAutomationServer.server.Rooms)
                        {
                            if (obj.Name.ToLower().Equals(command[1].ToLower()))
                            {
                                room = (Room)obj;
                            }
                            if (Array.IndexOf(obj.FriendlyNames, command[1].ToLower()) > -1)
                            {
                                room = (Room)obj;
                            }
                        }
                        break;

                    case "R":
                        R = uint.Parse(command[1]);
                        break;

                    case "G":
                        G = uint.Parse(command[1]);
                        break;

                    case "B":
                        B = uint.Parse(command[1]);
                        break;

                    case "dimmer":
                        dimmer = int.Parse(command[1]);
                        break;

                    case "color":
                        color = command[1];
                        break;

                    case "percentage":
                        dimm_percentage = uint.Parse(command[1]);
                        break;

                    case "switch":
                        status = command[1];
                        break;
                }
            }
            if (status != null)
            {
                room.Switch(bool.Parse(status));
                return "";
            }
            if (color != null)
            {
                uint[] vls = ColorConverter.ConvertNameToRGB(color);
                room.Color(vls[0], vls[1], vls[2], dimmer);
                return "";
            }
            if (dimm_percentage != 400)
            {
                room.Dimm(dimm_percentage, dimmer);
                return "";
            }
            room.Color(R, G, B, dimmer);
            return "";
        }
    }
}
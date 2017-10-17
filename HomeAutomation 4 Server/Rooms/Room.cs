using HomeAutomation.Dictionaries;
using HomeAutomation.Network;
using HomeAutomation.Network.APIStatus;
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
        public Room()
        {
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
                if (item is ISwitch)
                {
                    if (status) ((ISwitch)item).Start(); else ((ISwitch)item).Stop();
                }
                /*//Console.WriteLine("switching_pre");
                if (item.GetObjectType().Equals("LIGHT_GPIO_RGB"))
                {
                    //Console.WriteLine("switching");
                    ((ILight)item).Pause(status);
                    Thread.Sleep(1000);
                 }
                else if (item.GetObjectType().Equals("GENERIC_SWITCH"))
                {
                    if (status) ((ISwitch)item).Start(); else ((ISwitch)item).Stop();
                }*/
            }
        }
        public void Color(uint R, uint G, uint B, int dimmer)
        {
            HomeAutomationServer.server.Telegram.Log("Changing color of room `" + this.Name + "`.");
            foreach (IObject item in Objects)
            {
                if (item.GetObjectType().Equals("LIGHT_GPIO_RGB"))
                {
                        ((IColorableLight)item).Set(R, G, B, dimmer);
                        Thread.Sleep(1000);
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
                if (item.GetObjectType().Equals("LIGHT_GPIO_RGB") || item.GetObjectType().Equals("LIGHT_GPIO_W"))
                {
                    ((ILight)item).Dimm(percentace, dimmer);
                    Thread.Sleep(1000);
                }
            }
        }
        private static Room FindRoomFromName(string name)
        {
            Room room = null;
            foreach (Room obj in HomeAutomationServer.server.Rooms)
            {
                if (obj.Name.ToLower().Equals(name.ToLower()))
                {
                    room = (Room)obj;
                }
                if (Array.IndexOf(obj.FriendlyNames, name.ToLower()) > -1)
                {
                    room = (Room)obj;
                }
            }
            return room;
        }
        public static string SendParameters(string method, string[] request)
        {
            if (method.Equals("changeColor/RGB"))
            {
                Room room = null;
                uint R = 0;
                uint G = 0;
                uint B = 0;
                int dimmer = 0;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            room = FindRoomFromName(command[1]);
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
                    }
                    if (room == null) return "ADD ERROR API";
                }
                room.Color(R, G, B, dimmer);
                return "ADD STATUS API";
            }

            if (method.Equals("changeColor/name"))
            {
                return "NOT IMPLEMENTED";
            }

            if (method.Equals("switch"))
            {
                Room room = null;
                bool status = false;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            room = FindRoomFromName(command[1]);
                            break;
                        case "switch":
                            status = bool.Parse(command[1]);
                            break;
                    }
                    if (room == null) return "ADD ERROR API";
                }
                room.Switch(status);
                return "";
            }
            if (method.Equals("dimm"))
            {
                Room room = null;
                byte dimm_percentage = 255;
                int dimmer = 0;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    switch (command[0])
                    {
                        case "objname":
                            room = FindRoomFromName(command[1]);
                            break;
                        case "percentage":
                            dimm_percentage = byte.Parse(command[1]);
                            break;
                        case "dimmer":
                            dimmer = int.Parse(command[1]);
                            break;
                    }
                    if (room == null) return "ADD ERROR API";
                }
                room.Dimm(dimm_percentage, dimmer);
                return "";
            }
            if (method.Equals("createRoom"))
            {
                string name = null;
                bool hiddenRoom = false;
                string[] friendlyNames = null;

                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    if (command[0].Equals("interface")) continue;
                    switch (command[0])
                    {
                        case "objname":
                            name = command[1];
                            break;

                        case "setfriendlynames":
                            string names = command[1];
                            friendlyNames = names.Split(',');
                            break;

                        case "hiddenroom":
                            string hiddenroomString = command[1];
                            hiddenRoom = bool.Parse(hiddenroomString);
                            break;
                    }
                }
                Room editRoom = null;
                foreach (Room area in HomeAutomationServer.server.Rooms)
                {
                    if (area.Name.ToLower().Equals(name.ToLower()))
                    {
                        editRoom = area;
                    }
                }
                if (editRoom != null)
                {
                    editRoom.Name = name;
                    if (friendlyNames != null) editRoom.FriendlyNames = friendlyNames;
                    editRoom.Hidden = hiddenRoom;
                    ReturnStatus return_data = new ReturnStatus(CommonStatus.SUCCESS);
                    return_data.Object.room = editRoom;
                    return return_data.Json();
                }
                Room room = new Room(name, friendlyNames, hiddenRoom);
                ReturnStatus data = new ReturnStatus(CommonStatus.SUCCESS);
                data.Object.room = room;
                return data.Json();
            }
            if (string.IsNullOrEmpty(method))
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
            }
            return "";
        }
    }
}
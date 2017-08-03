using HomeAutomation.Objects;
using HomeAutomation.Rooms;
using HomeAutomationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Network.Objects
{
    class FindInterfaces
    {
        NetworkInterface inter;

        public FindInterfaces()
        {
            this.inter = new NetworkInterface("auto", Handler);
        }

        public static void Handler(string[] request)
        {
            foreach (string cmd in request)
            {
                string[] command = cmd.Split('=');
                if (command[0].Equals("interface")) continue;
                switch (command[0])
                {
                    case "objname":

                        foreach (Room room in HomeAutomationServer.server.Rooms)
                        {
                            if (room.Name.Equals(command[1]))
                            {
                                Room.SendParameters(request);
                            }
                            if (Array.IndexOf(room.FriendlyNames, command[1].ToLower()) > -1)
                            {
                                Room.SendParameters(request);
                            }
                        }
                        foreach (IObject obj in HomeAutomationServer.server.Objects)
                        {
                            if (obj.GetName().Equals(command[1]))
                            {
                                obj.GetInterface().Run(request);
                            }
                            if (obj.GetFriendlyNames() == null) continue;
                            if (Array.IndexOf(obj.GetFriendlyNames(), command[1].ToLower()) > -1)
                            {
                                obj.GetInterface().Run(request);
                            }

                        }
                        break;
                }
            }
        }
    }
}
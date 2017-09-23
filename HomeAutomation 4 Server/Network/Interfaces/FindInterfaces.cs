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

        public static string Handler(string[] request)
        {
            string identity = null;
            string objname = null;
            NetworkInterface iobj = null;

            foreach (string cmd in request)
            {
                string[] command = cmd.Split('=');
                if (command[0].Equals("interface")) continue;
                switch (command[0])
                {
                    case "objname":
                        objname = command[1];
                        break;

                    case "identity":
                        identity = command[1];
                        break;
                }
            }
            if (identity != null) objname = objname.Replace("my ", identity + " ");
            if (objname.StartsWith("the ")) objname = objname.Substring(4);
            foreach (Room room in HomeAutomationServer.server.Rooms)
            {
                if (room.Name.Equals(objname))
                {
                    iobj = NetworkInterface.FromId("room");
                }
                if (Array.IndexOf(room.FriendlyNames, objname.ToLower()) > -1)
                {
                    iobj = NetworkInterface.FromId("room");
                    //Room.SendParameters(request);
                }
            }
            foreach (IObject obj in HomeAutomationServer.server.Objects)
            {
                if (obj.GetName().Equals(objname))
                {
                    iobj = obj.GetInterface();
                }
                if (obj.GetFriendlyNames() == null) continue;
                if (Array.IndexOf(obj.GetFriendlyNames(), objname.ToLower()) > -1)
                {
                    iobj = obj.GetInterface();
                }

            }
            if (identity != null)
            {
                List<string> finalRequest = new List<string>();
                foreach (string cmd in request)
                {
                    string[] command = cmd.Split('=');
                    //if (command[0].Equals("interface")) continue;
                    switch (command[0])
                    {
                        case "objname":
                            finalRequest.Add(command[0] + "=" + objname);
                            break;
                        default:
                            finalRequest.Add(command[0] + "=" + command[1]);
                            break;
                    }
                }
                iobj.Run(finalRequest.ToArray());
            }
            else
            {
                iobj.Run(request);
            }
            return "";
        }
    }
}
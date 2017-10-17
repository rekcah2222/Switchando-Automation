using HomeAutomation.Objects;
using HomeAutomation.Objects.Switches;
using HomeAutomationCore;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Web;

namespace HomeAutomation.Network
{
    class HTTPHandler
    {
        HttpServer server;
        public HTTPHandler(string[] ip)
        {
            server = new HttpServer(SendResponse, ip);
            server.Run();
        }

        public static string SendResponse(HttpListenerRequest request)
        {            
            string url = request.Url.PathAndQuery.Substring(1);
            url = HttpUtility.UrlDecode(url);

            if (url.Contains("get=devices"))
            {
                return JsonConvert.SerializeObject(HomeAutomationServer.server.Objects);
            }
            if (url.Contains("get=switchabledevices"))
            {
                System.Collections.Generic.List<ISwitch> switchables = new System.Collections.Generic.List<ISwitch>();
                foreach (IObject iobj in HomeAutomationServer.server.Objects)
                {
                    if (iobj is ISwitch)
                    {
                        switchables.Add((ISwitch)iobj);
                    }
                }
                return JsonConvert.SerializeObject(switchables);
            }
            if (url.Contains("get=rooms"))
            {
                return JsonConvert.SerializeObject(HomeAutomationServer.server.Rooms);
            }
            if (url.Contains("get=clients"))
            {
                var json = JsonConvert.SerializeObject(HomeAutomationServer.server.Clients);
                return json;
            }

            //CHECK PASSWORD
            string[] interfaceMethod = url.Split('/');
            string netInterface = null;
            string methodsRaw;
            netInterface = interfaceMethod[0].ToLower();
            methodsRaw = url.Substring(netInterface.Length + 1);
            string method = methodsRaw.Split('?')[0];
            string[] parameters = methodsRaw.Split('?')[1].Split('&');

            foreach (NetworkInterface networkInterface in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (networkInterface.Id.ToLower().Equals(netInterface))
                {
                    string returnMessage = networkInterface.Run(method, parameters);
                    File.WriteAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/configuration.json", JsonConvert.SerializeObject(HomeAutomationServer.server.Rooms));
                    HomeAutomationServer.server.ObjectNetwork.Save();
                    return returnMessage;
                }
            }

            string message = request.Url.Query.Substring(1);
            message = HttpUtility.UrlDecode(message);
            Console.WriteLine(message);

            if (!message.Contains("&password=" + HomeAutomationServer.server.GetPassword())) return "INVALID PASSWORD";

            

            string[] commands = message.Split('&');

            string[] icommand = commands[0].Split('=');
            if (icommand[0].Equals("interface"))
            {
                foreach (NetworkInterface networkInterface in HomeAutomationServer.server.NetworkInterfaces)
                {
                    if (networkInterface.Id.Equals(icommand[1]))
                    {
                        return networkInterface.Run(null, commands);
                    }
                }
            }
            else if (icommand[0].Equals("objname"))
            {
                return NetworkInterface.FromId("auto").Run(null, commands);
            }

            return string.Format("<HTML><BODY>HomeAutomation 4 is running!<br />" + request.Url.Query + "<br />{0}</BODY></HTML>", DateTime.Now);
        }
    }
}

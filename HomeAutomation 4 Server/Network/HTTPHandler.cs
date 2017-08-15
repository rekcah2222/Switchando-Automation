using HomeAutomation.Objects;
using HomeAutomation.Objects.Switches;
using HomeAutomationCore;
using Newtonsoft.Json;
using System;
using System.Net;
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
            string message = request.Url.Query.Substring(1);
            message = HttpUtility.UrlDecode(message);
            Console.WriteLine(message);

            if (!message.Contains("&password=" + HomeAutomationServer.server.GetPassword())) return "INVALID PASSWORD";

            if (message.Contains("get=devices"))
            {
                return JsonConvert.SerializeObject(HomeAutomationServer.server.Objects);
            }
            if (message.Contains("get=switchabledevices"))
            {
                System.Collections.Generic.List<ISwitch> switchables = new System.Collections.Generic.List<ISwitch>();
                foreach(IObject iobj in HomeAutomationServer.server.Objects)
                {
                    if (iobj is ISwitch)
                    {
                        switchables.Add((ISwitch)iobj);
                    }
                }
                return JsonConvert.SerializeObject(switchables);
            }
            if (message.Contains("get=rooms"))
            {
                return JsonConvert.SerializeObject(HomeAutomationServer.server.Rooms);
            }
            if (message.Contains("get=clients"))
            {
                var json = JsonConvert.SerializeObject(HomeAutomationServer.server.Clients);
                return json;
            }

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

            return string.Format("<HTML><BODY>HomeAutomation 4 is running!<br />" + request.Url.Query + "<br />{0}</BODY></HTML>", DateTime.Now);
        }
    }
}

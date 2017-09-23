using HomeAutomationCore;
using System;

namespace HomeAutomation.Network
{
    public class NetworkInterface
    {
        public string Id;

        public delegate string Delegate(string[] request);
        Delegate Handler;

        public NetworkInterface(string id, Delegate handler)
        {
            this.Id = id;
            this.Handler = handler;
            Console.WriteLine("registering " + id);
            HomeAutomationServer.server.NetworkInterfaces.Add(this);
        }

        public string Run(string[] request)
        {
            return this.Handler(request);
        }

        public static NetworkInterface FromId(string id)
        {
            foreach(NetworkInterface networkInterface in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (id.StartsWith(networkInterface.Id))
                {
                    return networkInterface;
                }
            }
            return null;
        }
    }
}
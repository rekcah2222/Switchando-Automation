using HomeAutomationCore;

namespace HomeAutomation.Network
{
    public class NetworkInterface
    {
        public string Id;

        public delegate void Delegate(string[] request);
        Delegate Handler;

        public NetworkInterface(string id, Delegate handler)
        {
            this.Id = id;
            this.Handler = handler;

            HomeAutomationServer.server.NetworkInterfaces.Add(this);
        }

        public void Run(string[] request)
        {
            this.Handler(request);
        }

        public static NetworkInterface FromId(string id)
        {
            foreach(NetworkInterface networkInterface in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (networkInterface.Id.Equals(id))
                {
                    return networkInterface;
                }
            }
            return null;
        }
    }
}
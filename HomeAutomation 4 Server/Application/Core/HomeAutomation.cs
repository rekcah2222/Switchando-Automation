using HomeAutomation.Application.ConfigRetriver;
using HomeAutomation.ConfigRetriver;
using HomeAutomation.Logging.Telegram;
using HomeAutomation.Network;
using HomeAutomation.ObjectInterfaces;
using HomeAutomation.Objects;
using HomeAutomation.Rooms;
using HomeAutomation.Scenarios;
using HomeAutomation.Users;
using System.Collections.Generic;

namespace HomeAutomationCore
{
    public class HomeAutomationServer
    {
        public static HomeAutomationServer server;

        public List<Client.Client> Clients { get; set; }
        public List<NetworkInterface> NetworkInterfaces { get; set; }
        public List<SetupTool> Setups { get; set; }
        public List<Configuration> Configs { get; set; }
        public List<Identity> Identities { get; set; }

        public List<IObject> Objects { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Scenario> Scenarios { get; set; }

        public TelegramBot Telegram { get; set; }
        
        public ObjectNetwork ObjectNetwork { get; set; }

        public string House;
        private string Password;

        public HomeAutomationServer(string house, string password)
        {
            server = this;
            this.House = house;
            this.Password = password;

            Clients = new List<Client.Client>();
            Objects = new List<IObject>();
            Rooms = new List<Room>();
            //Telegram = new TelegramBot(null);

            NetworkInterfaces = new List<NetworkInterface>();
            Setups = new List<SetupTool>();
            Configs = new List<Configuration>();

            ObjectNetwork = new ObjectNetwork();
            ObjectNetwork.MethodInterfaces = new List<MethodInterface>();
            ObjectNetwork.ObjectInterfaces = new List<ObjectInterface>();

        }
        public string GetPassword()
        {
            return this.Password;
        }
        public void SetPassword(string password)
        {
            this.Password = password;
        }
    }
}
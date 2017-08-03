using HomeAutomation.Logging.Telegram;
using HomeAutomation.Network;
using HomeAutomation.Objects;
using HomeAutomation.Rooms;
using HomeAutomation.Scenarios;
using HomeAutomation.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomationCore
{
    public class HomeAutomationServer
    {
        public static HomeAutomationServer server;

        public List<Client.Client> Clients { get; set; }
        public List<NetworkInterface> NetworkInterfaces { get; set; }
        public List<Identity> Identities { get; set; }

        public List<IObject> Objects { get; set; }
        public List<Room> Rooms { get; set; }
        public List<Scenario> Scenarios { get; set; }

        public TelegramBot Telegram { get; set; }

        public string House;
        public string Password;

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
        }
    }
}
using Homeautomation.GPIO;
using HomeAutomation.Application.ConfigRetriver;
using HomeAutomation.ConfigRetriver;
using HomeAutomation.Logging.Telegram;
using HomeAutomation.Network;
using HomeAutomation.Network.Objects;
using HomeAutomation.ObjectInterfaces;
using HomeAutomation.Objects;
using HomeAutomation.Objects.Blinds;
using HomeAutomation.Objects.External;
using HomeAutomation.Objects.External.Plugins;
using HomeAutomation.Objects.Fans;
using HomeAutomation.Objects.Inputs;
using HomeAutomation.Objects.Lights;
using HomeAutomation.Rooms;
using HomeAutomation.Scenarios;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;

namespace HomeAutomationCore
{
    static class HomeAutomationMain
    {
        static void Main(string[] args)
        {
            new HomeAutomationServer("A Switchando family", "password");

            //Console.WriteLine(PIGPIO.pigpio_start(null, null));

            Console.WriteLine("Welcome to Switchando Automation 4 BETA 4 (Bountiful Update) Server by Marco Realacci!");

            new Room();
            new NetworkInterface("GENERIC_SWITCH", Relay.SendParameters);
            new NetworkInterface("OBJECT_INTERFACE", ObjectInterface.SendParameters);
            new NetworkInterface("METHOD_INTERFACE", MethodInterface.SendParameters);

            new SetupTool("LIGHT_GPIO_RGB", RGBLight.Setup);
            new SetupTool("LIGHT_GPIO_W", WLight.Setup);
            new SetupTool("GENERIC_SWITCH", Relay.Setup);
            new SetupTool("BUTTON", Button.Setup);
            new SetupTool("SWITCH_BUTTON", SwitchButton.Setup);
            new SetupTool("EXTERNAL_SWITCH", WebRelay.Setup);
            new SetupTool("BLINDS", Blinds.Setup);



            TCPServer.StartListening();

            new ConfigRetriver();

            string prePsw = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/password.txt");
            HomeAutomationServer.server.SetPassword(prePsw.Trim());

            string jsonned = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/configuration.json");

            if (File.Exists(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/telegram.config"))
            {
                string telegramRaw = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/telegram.config"); //token@log@alert
                string[] telegramData = telegramRaw.Split('@');
                if (telegramData.Length == 3)
                {
                    HomeAutomationServer.server.Telegram = new TelegramBot(telegramData[0]);
                    HomeAutomationServer.server.Telegram.SetLogChat(long.Parse(telegramData[1]));
                    HomeAutomationServer.server.Telegram.SetAlertChat(long.Parse(telegramData[2]));
                }
            }
            Console.WriteLine(HomeAutomationServer.server.GetPassword());

            if (!string.IsNullOrEmpty(jsonned))
            {
                ModelRoom[] rooms = JsonConvert.DeserializeObject<ModelRoom[]>(jsonned);
                foreach (ModelRoom mRoom in rooms)
                {
                    Room room = new Room(mRoom.Name, mRoom.FriendlyNames, mRoom.Hidden);
                    foreach (dynamic device in mRoom.Objects)
                    {
                        Console.WriteLine(device.ClientName + " <<->> " + device.Name + " -> " + device.ObjectType.ToString());

                        Client.Client client = null;

                        bool toAdd = true;

                        if (device.ClientName != null)
                        {
                            foreach (Client.Client clnt in HomeAutomationServer.server.Clients)
                            {
                                if (clnt.Name.Equals(device.ClientName))
                                {
                                    client = clnt;
                                    toAdd = false;
                                }
                            }
                            if (toAdd) client = new Client.Client(null, 0, device.ClientName);

                            if (HomeAutomationServer.server.Clients.Count == 0)
                            {
                                client = new Client.Client(null, 0, device.ClientName);
                            }
                        }
                        bool exit = false;
                        foreach (IObject iobj in HomeAutomationServer.server.Objects)
                        {
                            if (iobj.GetName().Equals(device.Name))
                            {
                                room.AddItem(iobj);
                                exit = true;
                            }
                        }
                        if (exit) continue;

                        if (SetupTool.Exists(device.ObjectType))
                        {
                            device.Client = client;
                            SetupTool.FromId(device.ObjectType).Run(room, device);
                        }
                    }
                }
            }
            foreach (Room room in HomeAutomationServer.server.Rooms)
            {
                Console.WriteLine(room.Name + " -> ");
                foreach (IObject iobj in room.Objects)
                {
                    Console.WriteLine(iobj.GetName());
                }
                Console.WriteLine();
            }
            foreach (IObject iobj in HomeAutomationServer.server.Objects)
            {
                Console.WriteLine(iobj.GetName());
            }

            new Client.Client(null, 0, "local");
            new HTTPHandler(new string[] { "http://localhost:8080/api/", "http://localhost:8080/" });
            new FindInterfaces();
            //new VoiceInterface();

            Console.WriteLine("\n\n\n\n" + JsonConvert.SerializeObject(HomeAutomationServer.server.Rooms));
            Plugins.LoadAll("plugins");
            Console.ReadLine();
        }
    }
}
 
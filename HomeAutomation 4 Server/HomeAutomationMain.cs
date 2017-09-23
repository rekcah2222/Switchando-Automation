using Homeautomation.GPIO;
using HomeAutomation.Application.ConfigRetriver;
using HomeAutomation.ConfigRetriver;
using HomeAutomation.Logging.Telegram;
using HomeAutomation.Network;
using HomeAutomation.Network.Interfaces.Voice;
using HomeAutomation.Network.Objects;
using HomeAutomation.Objects;
using HomeAutomation.Objects.Blinds;
using HomeAutomation.Objects.External;
using HomeAutomation.Objects.External.Plugins;
using HomeAutomation.Objects.Fans;
using HomeAutomation.Objects.Inputs;
using HomeAutomation.Objects.Lights;
using HomeAutomation.Objects.Switches;
using HomeAutomation.Rooms;
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
            //Scenario.Load();
            Console.WriteLine(PIGPIO.pigpio_start(null, null));

            Console.WriteLine("Welcome to Switchando Automation 4.5 BETA BV3 (Developers Update) Server by Marco Realacci!");

            new SetupTool("LIGHT_GPIO_RGB", RGBLight.Setup);
            new SetupTool("LIGHT_GPIO_W", WLight.Setup);
            new SetupTool("GENERIC_SWITCH", Relay.Setup);
            new SetupTool("BUTTON", Button.Setup);
            new SetupTool("SWITCH_BUTTON", SwitchButton.Setup);
            new SetupTool("EXTERNAL_SWITCH", WebRelay.Setup);
            new SetupTool("BLINDS", Blinds.Setup);

            TCPServer.StartListening();

            new ConfigRetriver();

            //string jsonned = "[{\"Name\":\"Salotto\",\"FriendlyNames\":[\"living room\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":false},{\"Name\":\"Stanza di Marco\",\"FriendlyNames\":[\"marco's room\",\"marcos room\",\"marco's room devices\"],\"Objects\":[{\"ClientName\":\"marcoroom\",\"Name\":\"button 1\",\"Pin\":12,\"Commands\":[\"interface,,light_w,,,objname,,Striscia a led bianca,,,switch,,%EmulatedSwitchStatus%\",\"interface,,light_rgb,,,objname,,LED RGB,,,switch,,%EmulatedSwitchStatus%\"],\"ObjectType\":4},{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":false},{\"Name\":\"all_lights\",\"FriendlyNames\":[\"all lights\",\"house lights\",\"house's lights\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1},{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true},{\"Name\":\"Marco's lights\",\"FriendlyNames\":[\"marco's room lights\",\"lights in marco's room\",\"marcos room lights\"],\"Objects\":[{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true},{\"Name\":\"living room's lights\",\"FriendlyNames\":[\"living room lights\",\"lights in living room\",\"living room's lights\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true}]";
            //string jsonned = "";
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

                        /*if (device.ObjectType == "LIGHT_GPIO_RGB")
                        {
                            RGBLight light = new RGBLight();
                            light.PinR = device.PinR;
                            light.PinG = device.PinG;
                            light.PinB = device.PinB;
                            light.Name = device.Name;
                            light.FriendlyNames = device.FriendlyNames;
                            light.Description = device.Description;
                            light.Switch = device.Switch;
                            light.ValueR = device.ValueR;
                            light.ValueG = device.ValueG;
                            light.ValueB = device.ValueB;
                            light.ClientName = client.Name;
                            light.SetClient(client);

                            HomeAutomationServer.server.Objects.Add(light);
                            room.AddItem(light);
                            light.Init();
                        }
                        else if (device.ObjectType == "LIGHT_GPIO_W")
                        {
                            WLight light = new WLight();
                            light.Pin = device.Pin;
                            light.Name = device.Name;
                            light.FriendlyNames = device.FriendlyNames;
                            light.Description = device.Description;
                            light.Switch = device.Switch;
                            light.Value = device.Value;
                            light.ClientName = client.Name;
                            light.SetClient(client);

                            HomeAutomationServer.server.Objects.Add(light);
                            room.AddItem(light);
                            light.Init();
                        }


                        else if (device.ObjectType == "FAN")
                        {
                            SimpleFan fan = new SimpleFan();
                            fan.Pin = device.Pin;
                            fan.Name = device.Name;
                            fan.Description = device.Description;
                            fan.Enabled = device.Switch;
                            fan.ClientName = client.Name;
                            fan.SetClient(client);

                            HomeAutomationServer.server.Objects.Add(fan);
                            room.AddItem(fan);
                        }
                        else if (device.ObjectType == "BUTTON")
                        {
                            Button button = new Button(client, device.Name, device.Pin, device.IsRemote);
                            foreach (string command in device.Commands)
                            {
                                button.AddCommand(command);
                            }
                            foreach (string objectName in device.Objects)
                            {
                                button.AddObject(objectName);
                            }
                            button.ClientName = client.Name;
                            button.SetClient(client);
                            room.AddItem(button);
                        }
                        else if (device.ObjectType == "SWITCH_BUTTON")
                        {
                            SwitchButton button = new SwitchButton(client, device.Name, device.Pin, device.IsRemote);
                            foreach (string command in device.CommandsOn)
                            {
                                button.AddCommand(command, true);
                            }
                            foreach (string command in device.CommandsOff)
                            {
                                button.AddCommand(command, false);
                            }
                            foreach (string objectName in device.Objects)
                            {
                                button.AddObject(objectName);
                            }
                            button.ClientName = client.Name;
                            button.SetClient(client);
                            room.AddItem(button);
                        }
                        else if (device.ObjectType == "GENERIC_SWITCH")
                        {
                            Relay relay = new Relay();
                            relay.Pin = device.Pin;
                            relay.Name = device.Name;
                            relay.Description = device.Description;
                            relay.FriendlyNames = device.FriendlyNames;
                            relay.Enabled = device.Switch;
                            relay.ClientName = client.Name;
                            relay.SetClient(client);

                            HomeAutomationServer.server.Objects.Add(relay);
                            room.AddItem(relay);
                        }
                        else if (device.ObjectType == "EXTERNAL_SWITCH")
                        {
                            WebRelay relay = new WebRelay();
                            relay.Name = device.Name;
                            relay.Description = device.Description;
                            relay.Enabled = device.Switch;
                            relay.FriendlyNames = device.FriendlyNames;
                            relay.Online = device.Online;
                            relay.ID = device.ID;
                            
                            HomeAutomationServer.server.Objects.Add(relay);
                            room.AddItem(relay);
                        }
                        else if (device.ObjectType == "BLINDS")
                        {
                            ISwitch openDevice;
                            ISwitch closeDevice;
                            if (device.OpenDevice.ObjectType == "EXTERNAL_SWITCH")
                            {
                                WebRelay relay = new WebRelay();
                                relay.Name = device.OpenDevice.Name;
                                relay.Description = device.OpenDevice.Description;
                                relay.Enabled = device.OpenDevice.Switch;
                                relay.FriendlyNames = device.OpenDevice.FriendlyNames;
                                relay.Online = device.OpenDevice.Online;
                                relay.ID = device.ID;
                                openDevice = relay;

                                HomeAutomationServer.server.Objects.Add(relay);

                                relay = new WebRelay();
                                relay.Name = device.CloseDevice.Name;
                                relay.Description = device.CloseDevice.Description;
                                relay.Enabled = device.CloseDevice.Switch;
                                relay.FriendlyNames = device.CloseDevice.FriendlyNames;
                                relay.Online = device.CloseDevice.Online;
                                relay.ID = device.CloseDevice.ID;
                                closeDevice = relay;

                                HomeAutomationServer.server.Objects.Add(relay);
                            }
                            else
                            {
                                Relay relay = new Relay();
                                relay.Pin = device.OpenDevice.Pin;
                                relay.Name = device.OpenDevice.Name;
                                relay.Description = device.OpenDevice.Description;
                                relay.FriendlyNames = device.OpenDevice.FriendlyNames;
                                relay.Enabled = device.OpenDevice.Switch;
                                relay.ClientName = client.Name;
                                relay.SetClient(client);
                                openDevice = relay;

                                relay = new Relay();
                                relay.Pin = device.CloseDevice.Pin;
                                relay.Name = device.CloseDevice.Name;
                                relay.Description = device.CloseDevice.Description;
                                relay.FriendlyNames = device.CloseDevice.FriendlyNames;
                                relay.Enabled = device.CloseDevice.Switch;
                                relay.ClientName = client.Name;
                                relay.SetClient(client);
                                closeDevice = relay;
                            }
                        Blinds blinds = new Blinds(client, device.Name, openDevice, closeDevice, device.TotalSteps, device.Description, device.FriendlyNames);

                            room.AddItem(blinds);
                    }*/
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
            new HTTPHandler(new string[] { "http://*:8080/api/"});
            new FindInterfaces();
            new VoiceInterface();

            Console.WriteLine("\n\n\n\n" + JsonConvert.SerializeObject(HomeAutomationServer.server.Rooms));
            Plugins.LoadAll("plugins");
            Console.ReadLine();
        }
    }
}
 
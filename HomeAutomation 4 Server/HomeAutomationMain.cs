using Homeautomation.GPIO;
using HomeAutomation.ConfigRetriver;
using HomeAutomation.Logging.Telegram;
using HomeAutomation.Network;
using HomeAutomation.Network.Interfaces.Voice;
using HomeAutomation.Network.Objects;
using HomeAutomation.Objects;
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
    class HomeAutomationMain
    {
        static void Main(string[] args)
        {
            new HomeAutomationServer("Realacci");
            //Scenario.Load();
            //Console.WriteLine(PIGPIO.pigpio_start(null, null));

            Console.WriteLine("Welcome to HomeAutomation 4.0 BETA Server 0.6 by Marco Realacci!");
            TCPServer.StartListening();

            new ConfigRetriver();

            //string jsonned = "[{\"Name\":\"Salotto\",\"FriendlyNames\":[\"living room\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":false},{\"Name\":\"Stanza di Marco\",\"FriendlyNames\":[\"marco's room\",\"marcos room\",\"marco's room devices\"],\"Objects\":[{\"ClientName\":\"marcoroom\",\"Name\":\"button 1\",\"Pin\":12,\"Commands\":[\"interface,,light_w,,,objname,,Striscia a led bianca,,,switch,,%EmulatedSwitchStatus%\",\"interface,,light_rgb,,,objname,,LED RGB,,,switch,,%EmulatedSwitchStatus%\"],\"ObjectType\":4},{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":false},{\"Name\":\"all_lights\",\"FriendlyNames\":[\"all lights\",\"house lights\",\"house's lights\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1},{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true},{\"Name\":\"Marco's lights\",\"FriendlyNames\":[\"marco's room lights\",\"lights in marco's room\",\"marcos room lights\"],\"Objects\":[{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true},{\"Name\":\"living room's lights\",\"FriendlyNames\":[\"living room lights\",\"lights in living room\",\"living room's lights\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true}]";
            //string jsonned = "";
            string jsonned = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/configuration.json");

            if (!string.IsNullOrEmpty(jsonned))
            {
                ModelRoom[] rooms = JsonConvert.DeserializeObject<ModelRoom[]>(jsonned);
                foreach (ModelRoom mRoom in rooms)
                {
                    Room room = new Room(mRoom.Name, mRoom.FriendlyNames, mRoom.Hidden);
                    foreach (HomeAutomationModel device in mRoom.Objects)
                    {
                        Console.WriteLine(device.ClientName + " <<->> " + device.Name + " -> " + device.ObjectType.ToString());

                        Client.Client client = null;

                        bool toAdd = true;

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

                        if (device.ObjectType == HomeAutomationObject.LIGHT)
                        {
                            if (device.LightType == LightType.RGB_LIGHT)
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
                            else if (device.LightType == LightType.W_LIGHT)
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
                        }
                        else if (device.ObjectType == HomeAutomationObject.FAN)
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
                        else if (device.ObjectType == HomeAutomationObject.BUTTON)
                        {
                            Button button = new Button(client, device.Name, device.Pin);
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
                        else if (device.ObjectType == HomeAutomationObject.SWITCH_BUTTON)
                        {
                            SwitchButton button = new SwitchButton(client, device.Name, device.Pin);
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
                        else if (device.ObjectType == HomeAutomationObject.GENERIC_SWITCH)
                        {
                            Relay relay = new Relay();
                            relay.Pin = device.Pin;
                            relay.Name = device.Name;
                            relay.Description = device.Description;
                            relay.Enabled = device.Switch;
                            relay.ClientName = client.Name;
                            relay.SetClient(client);

                            HomeAutomationServer.server.Objects.Add(relay);
                            room.AddItem(relay);
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

            new HTTPHandler("http://localhost:8080/api/");

            //TelegramBot telegramBot = new TelegramBot("358568464:AAFQOSmt119n3VPPInaQxWXS95vxgqvaWT0");
            //telegramBot.SetLogChat(-180720312);
            //telegramBot.SetAlertChat(-180720312);


            /*Client.Client clientMrcrlc = new Client.Client(null, 0, "marcoroom");
            Client.Client clientLivingRoom = new Client.Client(null, 0, "livingroom");

            var livingRGB = new RGBLight(clientLivingRoom, "Retroilluminazione sul mobile", 20, 26, 19, "Striscia LED RGB dietro al mobile del salone", new string[] { "living room led", "living room's led" });
            var livingW = new WLight(clientLivingRoom, "Faretti sul mobile", 21, "Faretti sul mobile del salone", new string[] { "living room's spotlight", "living room spotlight" });
            //WLight greenTestSwitch = new WLight(clientLivingRoom, "pi zero W", 26, "pi zero W", new string[0]);
            //Relay redTestSwitch = new Relay(clientLivingRoom, "pi zero w red", 21, "pi zero w red", new string[0]);

            var btn = new Button(clientMrcrlc, "button 1", 12);
            btn.AddCommand("interface=light_w&objname=Striscia a led bianca&switch=%EmulatedSwitchStatus%");
            btn.AddCommand("interface=light_rgb&objname=LED RGB&switch=%EmulatedSwitchStatus%");


            //btn.AddCommand("interface=light_rgb&objname=LED RGB&switch=false", false);
            //btn.AddCommand("interface=light_w&objname=Striscia a led bianca&switch=false", false);

            var marcoRGB = new RGBLight(clientMrcrlc, "LED RGB", 20, 16, 21, "Striscia LED RGB", new string[] { "led di marco", "luci di marco", "marco's color light" });
            var marcoW = new WLight(clientMrcrlc, "Striscia a led bianca", 26, "Illuminazione a led della stanza di Marco", new string[] { "marco's light" });
            //var marcoW = new Relay(clientMrcrlc, "Striscia a led bianca", 19, "test", new string[]{ "test" });

            var salone = new Room("Salotto", new string[] { "living room" }, false);
            salone.AddItem(livingRGB);
            salone.AddItem(livingW);

            var stanzaMarco = new Room("Stanza di Marco", new string[] { "marco's room", "marcos room", "marco's room devices"}, false);
            stanzaMarco.AddItem(marcoRGB);
            stanzaMarco.AddItem(marcoW);
            //stanzaMarco.AddItem(greenTestSwitch);
            //stanzaMarco.AddItem(redTestSwitch);

            //var mrcChairSwitch = new SwitchButton(clientMrcrlc, "mrc-chair-switch", 20);
            //mrcChairSwitch.AddCommand("interface=light_w&objname=Striscia a led bianca&W=180&dimmer=1000", true);
            //mrcChairSwitch.AddCommand("interface=light_rgb&objname=LED RGB&R=230&G=117&B=255&dimmer=1000", true);
            //mrcChairSwitch.AddCommand("interface=light_w&objname=Striscia a led bianca&W=255&dimmer=1000", false);
            //mrcChairSwitch.AddCommand("interface=light_rgb&objname=LED RGB&R=255&G=255&B=255&dimmer=1000", false);

            //Keywords
            var lights = new Room("all_lights", new string[] {"all lights", "house lights", "house's lights"}, true);
            lights.AddItem(livingRGB);
            lights.AddItem(livingW);
            lights.AddItem(marcoRGB);
            lights.AddItem(marcoW);

            var luciStanzaMarco = new Room("Marco's lights", new string[] { "marco's room lights", "lights in marco's room", "marcos room lights"}, true);
            luciStanzaMarco.AddItem(marcoRGB);
            luciStanzaMarco.AddItem(marcoW);

            var luciSalotto = new Room("living room's lights", new string[] { "living room lights", "lights in living room", "living room's lights" }, true);
            luciStanzaMarco.AddItem(livingRGB);
            luciStanzaMarco.AddItem(livingW);

            new FindInterfaces();
            new VoiceInterface();

            

            Room figa = new Room("switches", new string[0], true);

            var btn = new Button(clientMrcrlc, "button 1", 12);
            btn.AddCommand("interface=light_w&objname=Striscia a led bianca&switch=%EmulatedSwitchStatus%");
            btn.AddCommand("interface=light_rgb&objname=LED RGB&switch=%EmulatedSwitchStatus%");
            figa.AddItem(btn);*/

            Console.WriteLine("\n\n\n\n" + JsonConvert.SerializeObject(HomeAutomationServer.server.Rooms));
            Console.ReadLine();
            foreach(IObject iobj in HomeAutomationServer.server.Objects)
            {
                if (iobj.GetObjectType() == HomeAutomationObject.BUTTON)
                {
                    ((Button)iobj).Tick(null, null);
                }
            }
        }
    }
}
 
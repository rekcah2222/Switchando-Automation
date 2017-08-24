using Homeautomation.GPIO;
using HomeAutomation.ConfigRetriver;
using HomeAutomation.Network;
using HomeAutomation.Network.Interfaces.Voice;
using HomeAutomation.Network.Objects;
using HomeAutomation.Objects;
using HomeAutomation.Objects.External;
using HomeAutomation.Objects.Fans;
using HomeAutomation.Objects.Inputs;
using HomeAutomation.Objects.Lights;
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
            new HomeAutomationServer("Realacci", "password");
            //Scenario.Load();
            //Console.WriteLine(PIGPIO.pigpio_start(null, null));

            Console.WriteLine("Welcome to HomeAutomation 4.0 BETA Server 0.6 by Marco Realacci!");
            TCPServer.StartListening();

            new ConfigRetriver();

            //string jsonned = "[{\"Name\":\"Salotto\",\"FriendlyNames\":[\"living room\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":false},{\"Name\":\"Stanza di Marco\",\"FriendlyNames\":[\"marco's room\",\"marcos room\",\"marco's room devices\"],\"Objects\":[{\"ClientName\":\"marcoroom\",\"Name\":\"button 1\",\"Pin\":12,\"Commands\":[\"interface,,light_w,,,objname,,Striscia a led bianca,,,switch,,%EmulatedSwitchStatus%\",\"interface,,light_rgb,,,objname,,LED RGB,,,switch,,%EmulatedSwitchStatus%\"],\"ObjectType\":4},{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":false},{\"Name\":\"all_lights\",\"FriendlyNames\":[\"all lights\",\"house lights\",\"house's lights\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1},{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true},{\"Name\":\"Marco's lights\",\"FriendlyNames\":[\"marco's room lights\",\"lights in marco's room\",\"marcos room lights\"],\"Objects\":[{\"ClientName\":\"marcoroom\",\"PinR\":20,\"PinG\":16,\"PinB\":21,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"LED RGB\",\"FriendlyNames\":[\"led di marco\",\"luci di marco\",\"marco's color light\"],\"Description\":\"Striscia LED RGB\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"marcoroom\",\"Pin\":26,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Striscia a led bianca\",\"FriendlyNames\":[\"marco's light\"],\"Description\":\"Illuminazione a led della stanza di Marco\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true},{\"Name\":\"living room's lights\",\"FriendlyNames\":[\"living room lights\",\"lights in living room\",\"living room's lights\"],\"Objects\":[{\"ClientName\":\"livingroom\",\"PinR\":20,\"PinG\":26,\"PinB\":19,\"ValueR\":255,\"ValueG\":255,\"ValueB\":255,\"Brightness\":100,\"Switch\":true,\"Name\":\"Retroilluminazione sul mobile\",\"FriendlyNames\":[\"living room led\",\"living room's led\"],\"Description\":\"Striscia LED RGB dietro al mobile del salone\",\"nolog\":false,\"ObjectType\":0,\"LightType\":0},{\"ClientName\":\"livingroom\",\"Pin\":21,\"Value\":255,\"Brightness\":100,\"PauseValue\":0,\"Switch\":true,\"Name\":\"Faretti sul mobile\",\"FriendlyNames\":[\"living room's spotlight\",\"living room spotlight\"],\"Description\":\"Faretti sul mobile del salone\",\"ObjectType\":0,\"LightType\":1}],\"Hidden\":true}]";
            //string jsonned = "";
            HomeAutomationServer.server.SetPassword(File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/password.txt"));
            string jsonned = File.ReadAllText(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "/configuration.json");

            Console.WriteLine(HomeAutomationServer.server.GetPassword());

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
                        else if (device.ObjectType == HomeAutomationObject.SWITCH_BUTTON)
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
                        else if (device.ObjectType == HomeAutomationObject.GENERIC_SWITCH)
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
                        else if (device.ObjectType == HomeAutomationObject.EXTERNAL_SWITCH)
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
 
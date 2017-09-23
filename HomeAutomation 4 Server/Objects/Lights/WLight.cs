using Homeautomation.GPIO;
using HomeAutomation.Network;
using HomeAutomation.Rooms;
using HomeAutomationCore;
using HomeAutomationCore.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace HomeAutomation.Objects.Lights
{
    class WLight : ILight
    {
        Client Client;
        public string ClientName;

        public uint Pin;
        public uint Value, Brightness;
        public uint PauseValue;
        public bool Switch;

        public string Name;
        public string[] FriendlyNames;
        public string Description;

        public string ObjectType = "LIGHT_GPIO_W";
        //public LightType LightType = LightType.W_LIGHT;

        public WLight()
        {
            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("light_w")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("light_w", requestHandler);
        }
        public WLight(Client client, string Name, uint pin, string description, string[] FriendlyNames)
        {
            this.Client = client;
            this.ClientName = client.Name;
            this.Switch = true;
            this.Description = description;
            this.Name = Name;
            this.Pin = pin;
            this.Value = 255;
            this.Brightness = 100;
            this.FriendlyNames = FriendlyNames;

            if (Client.Name.Equals("local"))
            {
                PIGPIO.set_PWM_dutycycle(Client.PigpioID, Pin, Value);

                PIGPIO.set_PWM_frequency(Client.PigpioID, Pin, 4000);
            }

            HomeAutomationServer.server.Objects.Add(this);

            foreach (NetworkInterface netInt in HomeAutomationServer.server.NetworkInterfaces)
            {
                if (netInt.Id.Equals("light_w")) return;
            }
            NetworkInterface.Delegate requestHandler;
            requestHandler = SendParameters;
            NetworkInterface networkInterface = new NetworkInterface("light_w", requestHandler);
        }
        public void SetClient(Client client)
        {
            this.Client = client;
        }

        public void Set(uint value, int dimmer, bool nolog = false)
        {
            Console.WriteLine("Setting " + this.Name + " from " + this.Value + " to " + value + " with a dimmer of " + dimmer + "ms.");
            //if (!nolog) HomeAutomationServer.server.Telegram.Log("Setting " + this.Name + " from " + this.Value + " to " + value + " with a dimmer of " + dimmer + "ms.");
            this.Brightness = 100;
            if (value == this.Value) return;

            if (value == 0) Switch = true; else Switch = false;

            if (!Client.Name.Equals("local"))
            {
                UploadValues(value, dimmer);
                this.Value = value;
                return;
            }

            if (dimmer == 0)
            {
                PIGPIO.set_PWM_dutycycle(Client.PigpioID, Pin, value);
                this.Value = value;
                return;
            }

            double[] values = new double[4];
            values[0] = this.Value;
            values[1] = value;
            values[2] = this.Pin;
            values[3] = ((dimmer / (((int)this.Value) - (int)value)));
            //if (((this.Value * 255) - (value * 255)) == 0) values[3] = 0;
            DimmerThread(values);

            this.Value = value;
        }

        public void Dimm(uint percentage, int dimmer)
        {
            var W = Value;

            if (Brightness == 0)
            {
                Set(255, dimmer);
            }
            else
            {
                W = W * percentage / Brightness;
                Set(W, dimmer);
            }

            this.Brightness = percentage;
        }

        public async void DimmerThread(object data)
        {
            await Task.Delay(1);
            double[] values = (double[])data;
            int led = (int)values[2];
            if (values[3] < 0) values[3] *= -1;
            if (values[0] <= values[1])
            {
                for (double i = values[0]; i <= values[1]; i = i + 1)
                {
                    PIGPIO.set_PWM_dutycycle(Client.PigpioID, (uint)led, (uint)i);
                    if (values[3] == 0) values[3] = 1;
                    Thread.Sleep((int)values[3]);
                }
            }
            else
            {
                for (double i = values[0]; i >= values[1]; i = i - 1)
                {
                    PIGPIO.set_PWM_dutycycle(Client.PigpioID, (uint)led, (uint)i);
                    if (values[3] == 0) values[3] = 1;
                    Thread.Sleep((int)values[3]);
                }
            }
            PIGPIO.set_PWM_dutycycle(Client.PigpioID, (uint)led, (uint)values[1]);
        }

        void Block(long durationTicks)
        {
            Stopwatch sw;
            sw = Stopwatch.StartNew();
            int i = 0;

            while (sw.ElapsedTicks <= durationTicks)
            {
                if (sw.Elapsed.Ticks % 100 == 0)
                {
                    i++;
                }
            }
            sw.Stop();
        }

        public void Pause()
        {
            if (Switch)
            {
                //PauseValue = Value;
                Set(0, 1000);
                return;
            }
            else
            {
                Set(255, 1000);
                /*if (PauseValue == 0)
                {
                    
                    Set(255, 1000);
                    return;
                }
                else
                {
                    
                    Set(PauseValue, 1000);
                    return;
                }*/
            }
        }

        public void Pause(bool status)
        {
            if (!status)
            {
                //PauseValue = Value;
                Set(0, 1000);
                return;
            }
            else
            {
                Set(255, 1000);
                /*if (PauseValue == 0)
                {
                    
                    Set(255, 1000);
                    return;
                }
                else
                {
                    
                    Set(PauseValue, 1000);
                    return;
                }*/
            }
        }
        public void Start()
        {
            Pause(true);
        }
        public void Stop()
        {
            Pause(false);
        }
        public bool IsOn()
        {
            return Switch;
        }
        public LightType GetLightType()
        {
            return LightType.W_LIGHT;
        }
        public new string GetObjectType()
        {
            return "LIGHT";
        }
        public string GetName()
        {
            return Name;
        }
        public string[] GetFriendlyNames()
        {
            return FriendlyNames;
        }
        public uint GetValue()
        {
            return Value;
        }
        public NetworkInterface GetInterface()
        {
            return NetworkInterface.FromId("light_w");
        }
        void UploadValues(uint Value, int DimmerIntervals)
        {
            Client.Sendata("interface=light_w&objname=" + this.Name + "&W=" + Value + "&dimmer=" + DimmerIntervals);
        }
        public static string SendParameters(string[] request)
        {
            WLight light = null;
            uint Value = 0;
            int dimmer = 0;
            uint dimm_percentage = 400;
            bool nolog = false;
            string status = null;
            foreach (string cmd in request)
            {
                string[] command = cmd.Split('=');
                if (command[0].Equals("interface")) continue;
                
                switch (command[0])
                {
                    case "objname":
                        foreach (IObject obj in HomeAutomationServer.server.Objects)
                        {
                            if (obj.GetName().Equals(command[1]))
                            {
                                light = (WLight)obj;
                            }
                            if (obj.GetFriendlyNames() == null) continue;
                            if (Array.IndexOf(obj.GetFriendlyNames(), command[1].ToLower()) > -1)
                            {
                                light = (WLight)obj;
                            }
                        }
                        break;

                    case "W":
                        Value = uint.Parse(command[1]);
                        break;

                    case "dimmer":
                        dimmer = int.Parse(command[1]);
                        break;

                    case "percentage":
                        dimm_percentage = uint.Parse(command[1]);
                        break;

                    case "nolog":
                        nolog = true;
                        break;

                    case "switch":
                        status = command[1];
                        break;
                }
            }
            if (status != null)
            {

                light.Pause(bool.Parse(status));
                return "";
            }
            if (dimm_percentage != 400)
            {
                light.Dimm(dimm_percentage, dimmer);
                return "";
            }
            light.Set(Value, dimmer, nolog);
            return "";
        }
        public void Init()
        {
            if (Client.Name.Equals("local"))
            {
                PIGPIO.set_PWM_dutycycle(0, Pin, Value);
                PIGPIO.set_PWM_frequency(0, Pin, 4000);
            }
        }
        public static void Setup(Room room, dynamic device)
        {
            WLight light = new WLight();
            light.Pin = (uint)device.Pin;
            light.Name = device.Name;
            //light.FriendlyNames = ((List<object>)device.FriendlyNames).ToArray().Where(x => x != null)
            //.Select(x => x.ToString())
            //.ToArray();
            light.FriendlyNames = Array.ConvertAll(((List<object>)device.FriendlyNames).ToArray(), x => x.ToString());
            light.Description = device.Description;
            light.Switch = device.Switch;
            light.Value = (uint)device.Value;
            light.ClientName = device.Client.Name;
            light.SetClient(device.Client);

            HomeAutomationServer.server.Objects.Add(light);
            room.AddItem(light);
            light.Init();
        }
    }
}

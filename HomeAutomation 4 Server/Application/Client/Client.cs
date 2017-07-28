using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomationCore.Client
{
    public class Client
    {
        private string IP;
        public int PigpioID;
        public string Name;
        //public TcpClient TcpClient = null;
        private StreamWriter Writer = null;
        public bool Connected { get; set; }

        public Client(string ip, int pigpioID, string name)
        {
            this.Name = name;
            this.IP = ip;
            this.PigpioID = pigpioID;

            HomeAutomationServer.server.Clients.Add(this);
        }
        public void Connect(TcpClient client, StreamWriter writer)
        {
            //this.TcpClient = client;
            this.Writer = writer;
            this.Connected = true;
        }
        public void Sendata(string message)
        {
            if (!Connected) return;
            //var writer = new StreamWriter(TcpClient.GetStream()) { AutoFlush = true };
            try
            {
                Writer.WriteLine(message);
                Writer.Flush();
            }
            catch
            {
                Connected = false;
            }
            //writer.Close();
        }
    }
}
using HomeAutomation.Logging.Telegram;
using HomeAutomationCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Logging
{
    public static class Logger
    {
        public static void Log(string msg)
        {
            HomeAutomationServer.server.Telegram.Log(msg);
        }
        public static void Alert(string msg)
        {
            HomeAutomationServer.server.Telegram.Alert(msg);
        }
    }
}

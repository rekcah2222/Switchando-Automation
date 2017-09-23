using HomeAutomation.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Objects
{
    public interface IObject
    {
        string GetName();
        string GetObjectType();
        NetworkInterface GetInterface();
        string[] GetFriendlyNames();
    }
    public enum HomeAutomationObjectBackup
    {
        LIGHT,
        FAN,
        GENERIC_SWITCH,
        ROOM,
        BUTTON,
        SWITCH_BUTTON,
        EXTERNAL_SWITCH,
        BLINDS
    }
}

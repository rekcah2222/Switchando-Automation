using HomeAutomation.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.Objects.Switches
{
    public interface ISwitch : IObject
    {
        void Start();
        void Stop();
        bool IsOn();
    }
}
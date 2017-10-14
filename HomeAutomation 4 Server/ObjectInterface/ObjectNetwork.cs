using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.ObjectInterfaces
{
    public class ObjectNetwork
    {
        public List<MethodInterface> MethodInterfaces { get; set; }
        public List<ObjectInterface> ObjectInterfaces { get; set; }
        public List<Action> Actions { get; set; }
    }
}

using HomeAutomation.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HomeAutomation.ObjectInterfaces
{
    public class Condition
    {
        public ObjectInterface Property;
        public object Value;
        public string Device;
        private IObject SwitchandoObject;
        public Condition(string device, ObjectInterface property, object value)
        {
            foreach(IObject iobj in HomeAutomationCore.HomeAutomationServer.server.Objects)
            {
                if (iobj.GetName().Equals(device))
                {
                    SwitchandoObject = iobj;
                }
            }
            this.Property = property;
            this.Value = value;
            this.Device = device;
        }
        public bool Verify()
        {
            if (ObjectInterface.GetPropertyValue(SwitchandoObject, Property.Name).GetType().Equals(Property.Type))
            {
                if (ObjectInterface.GetPropertyValue(SwitchandoObject, Property.Name).Equals(Value))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
    }
}

using System.Collections.Generic;

namespace HomeAutomation.ConfigRetriver
{
    class ModelRoom
    {
        public string Name;
        public string[] FriendlyNames;
        public List<HomeAutomationModel> Objects;
        public bool Hidden;
    }
}

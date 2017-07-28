using HomeAutomationCore;

namespace HomeAutomation.Users
{
    public class Identity
    {
        public string Name;
        public Identity(string name)
        {
            this.Name = name;

            HomeAutomationServer.server.Identities.Add(this);
        }
        public string GetName()
        {
            return Name;
        }
    }
}
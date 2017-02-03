namespace LogManagement.Registration
{
    public interface ISystemRegistration
    {
        ISystemRegistration RegisterApplication(IApplicationRegistration application);
    }

    public class SystemRegistration : ISystemRegistration
    {
        private string _systemName = string.Empty;

        public SystemRegistration(string systemName)
        {
            _systemName = systemName;
        }

        public ISystemRegistration RegisterApplication(IApplicationRegistration application)
        {
            return this;
        }
    }
}

using System;

namespace LogManagement.Registration
{
    public interface IApplicationRegistration
    {
        IApplicationRegistration RegisterComponent<T>(IComponentRegistration<T> component);
    }

    public class ApplicationRegistration : IApplicationRegistration
    {
        string _applicationName = String.Empty;

        public ApplicationRegistration(string applicationName)
        {
            _applicationName = applicationName;
        }

        public IApplicationRegistration RegisterComponent<T>(IComponentRegistration<T> component)
        {
            throw new NotImplementedException();

            return this;
        }
    }
}

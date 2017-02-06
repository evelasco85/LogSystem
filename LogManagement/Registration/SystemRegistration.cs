using System;
using System.Collections.Generic;
using System.Linq;

namespace LogManagement.Registration
{
    public interface ISystemRegistration : IRegistration
    {
        IList<IComponentRegistration> GetComponents(string className);
        ISystemRegistration RegisterApplication(IApplicationRegistration application);
    }

    public class SystemRegistration : Registration, ISystemRegistration
    {
        IDictionary<string, IApplicationRegistration> _registrationContainers = new Dictionary<string, IApplicationRegistration>();


        public SystemRegistration(string systemName) : base(systemName)
        {
        }

        public ISystemRegistration RegisterApplication(IApplicationRegistration application)
        {
            if ((application == null) || (string.IsNullOrEmpty(application.Name)))
                throw new ArgumentException("'application' instance and 'application name' is required");

            if (_registrationContainers.ContainsKey(application.Name))
                _registrationContainers.Remove(application.Name);

            application.SystemRegistration = this;

            _registrationContainers.Add(application.Name, application);

            return this;
        }

        public IList<IComponentRegistration> GetComponents(string className)
        {
            IList<IComponentRegistration> components = _registrationContainers
                .SelectMany(kvp => kvp.Value.GetComponents(className))
                .ToList();

            return components;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using LogManagement.Registration;
using LogManagement.Services;

namespace LogManagement
{
    public interface IActivityMonitoring
    {
        void AddSystem(ISystemRegistration system);
        void ValidateCurrentCall<TCallingInstance>(TCallingInstance instance, string memberName);
    }

    public class ActivityMonitoring : IActivityMonitoring
    {
        private IDictionary<string, ISystemRegistration> _registrationContainers = new Dictionary<string, ISystemRegistration>();

        public void AddSystem(ISystemRegistration system)
        {
            if((system == null) || (string.IsNullOrEmpty(system.Name)))
                throw new ArgumentException("'system' instance and 'System Name' is required");

            if (_registrationContainers.ContainsKey(system.Name))
                _registrationContainers.Remove(system.Name);

            _registrationContainers.Add(system.Name, system);
        }

        public void ValidateCurrentCall<TCallingInstance>(TCallingInstance instance, string memberName)
        {
            string className = RegistrationService.GetInstance().GetClassName<TCallingInstance>();

            IList<IComponentRegistration> components = _registrationContainers
                .SelectMany(kvp => kvp.Value.GetComponents(className))
                .ToList();

            for (int index = 0; index < components.Count; index++)
            {
                IComponentRegistration component = components[index];
                IApplicationRegistration application = component.Application;
                ISystemRegistration system = application.SystemRegistration;

                
            }
        }
    }
}

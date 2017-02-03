using System;
using System.Collections.Generic;

namespace LogManagement.Registration
{
    public interface IApplicationRegistration
    {
        IApplicationRegistration RegisterComponent<T>(IComponentRegistration<T> component);
    }

    public class ApplicationRegistration : IApplicationRegistration
    {
        string _applicationName = String.Empty;
        IDictionary<string, IComponentRegistration> _componentDictionary = new Dictionary<string, IComponentRegistration>();

        public ApplicationRegistration(string applicationName)
        {
            _applicationName = applicationName;
        }

        public IApplicationRegistration RegisterComponent<T>(IComponentRegistration<T> component)
        {
            if (component == null)
                return this;

            if (_componentDictionary.ContainsKey(component.Identifier))
                _componentDictionary.Remove(component.Identifier);

            _componentDictionary.Add(component.Identifier, component);

            return this;
        }
    }
}

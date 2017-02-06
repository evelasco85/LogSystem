using System;
using System.Collections.Generic;
using System.Linq;

namespace LogManagement.Registration
{
    public interface IApplicationRegistration : IRegistration
    {
        ISystemRegistration SystemRegistration { get; set; }
        IList<IComponentRegistration> GetComponents(string className);
        IApplicationRegistration RegisterComponent<T>(IComponentRegistration<T> component);
    }

    public class ApplicationRegistration : Registration, IApplicationRegistration
    {
        private ISystemRegistration _systemRegistration;
        IDictionary<string, IComponentRegistration> _componentDictionary = new Dictionary<string, IComponentRegistration>();

        public ISystemRegistration SystemRegistration
        {
            get { return _systemRegistration; }
            set { _systemRegistration = value; }
        }

        public ApplicationRegistration(string name) : base(name)
        {
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

        public IList<IComponentRegistration> GetComponents(string className)
        {
            IList<IComponentRegistration> components = new List<IComponentRegistration>();

            components = _componentDictionary
                .Where(kvp => kvp.Value.MatchClass(className))
                .Select(kvp => kvp.Value)
                .ToList();

            return components;
        }
    }
}

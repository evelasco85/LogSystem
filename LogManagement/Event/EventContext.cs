using System;
using System.Collections.Generic;
using LogManagement.Event.Parameters;

namespace LogManagement.Event
{
    public interface IEventContext
    {
        void Assign(IEventVariable variable, object value);
        IEventVariable GetVariable(string variableName);
    }

    public class EventContext : IEventContext
    {
        IDictionary<string, IEventVariable> _contextData = new Dictionary<string, IEventVariable>();

        public void Assign(IEventVariable variable, object value)
        {
            if(variable == null)
                throw new ArgumentException("'variable' parameter is required");

            if (_contextData.ContainsKey(variable.Name))
                _contextData.Remove(variable.Name);

            variable.Value = value;

            _contextData.Add(variable.Name, variable);
        }

        public IEventVariable GetVariable(string variableName)
        {
            if(!_contextData.ContainsKey(variableName))
                throw new ArgumentException("Variable with name '{0}' is not found in current context", variableName);

            return _contextData[variableName];
        }
    }
}

using System;
using System.Collections.Generic;

namespace LogManagement.Event
{
    public interface IEventContext
    {
        IEventContext Assign(string variableName, object value);
        object GetVariable(string variableName);
        void Clear();
    }

    public class EventContext : IEventContext
    {
        IDictionary<string, object> _contextData = new Dictionary<string, object>();

        public IEventContext Assign(string variableName, object value)
        {
            if (string.IsNullOrEmpty(variableName))
                throw new ArgumentException("'variableName' parameter is required");

            if (!_contextData.ContainsKey(variableName))
            {
                _contextData.Add(variableName, value);
                return this;
            }

            _contextData[variableName] = value;

            return this;
        }

        public object GetVariable(string variableName)
        {
            if(!_contextData.ContainsKey(variableName))
                throw new ArgumentException(string.Format("Variable with name '{0}' is not found in current context", variableName));

            return _contextData[variableName];
        }

        public void Clear()
        {
            _contextData.Clear();
        }
    }
}

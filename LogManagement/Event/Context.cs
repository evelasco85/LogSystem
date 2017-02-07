using System;
using System.Collections.Generic;
using System.Linq;

namespace LogManagement.Event
{
    public interface IContext
    {
        IContext Assign(string variableName, object value);
        object GetVariable(string variableName);
        IEnumerable<string> GetVariableNameList();
        void Clear();
    }

    public class Context : IContext
    {
        IDictionary<string, object> _contextData = new Dictionary<string, object>();

        public IContext Assign(string variableName, object value)
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
            if (!_contextData.ContainsKey(variableName))
                return null;

            return _contextData[variableName];
        }

        public void Clear()
        {
            _contextData.Clear();
        }

        public IEnumerable<string> GetVariableNameList()
        {
            return _contextData.Keys;
        }
    }
}

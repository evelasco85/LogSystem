using System;

namespace LogManagement.Event.Parameters
{
    public class EventLiteral : EventData
    {
        private object _value;
        private string _name;

        public EventLiteral(string literalName, object value)
        {
            _value = value;
            _name = literalName;
        }

        public EventLiteral(object value) : this("Value", value)
        {
        }

        public override object GetData(IEventContext context)
        {
            return _value;
        }

        public override string GetSyntax(IEventContext context)
        {
            return string.Format("[{0} : {1}]", _name, Convert.ToString(GetData(context)));
        }
    }
}

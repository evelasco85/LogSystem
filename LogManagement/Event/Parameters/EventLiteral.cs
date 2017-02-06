using System;
using LogManagement.Event.Conditions;

namespace LogManagement.Event.Parameters
{
    public interface IEventLiteral
    {
    }

    public class EventLiteral : EventData, IEventLiteral
    {
        private object _value;

        public EventLiteral(object value)
        {
            _value = value;
        }

        public override object GetData(IEventContext context)
        {
            return _value;
        }
    }
}

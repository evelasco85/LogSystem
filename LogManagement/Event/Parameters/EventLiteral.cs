using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogManagement.Event.Conditions;

namespace LogManagement.Event.Parameters
{
    public interface IEventLiteral
    {
        
    }

    public class EventLiteral : EventBoolean, IEventLiteral
    {
        private object _value;

        public EventLiteral(object value)
        {
            _value = value;
        }

        public override bool Evaluate(IEventContext context)
        {
            return true;
        }
    }
}

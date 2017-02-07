using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogManagement.Event.Conditions;

namespace LogManagement.Event.Parameters
{
    public class EventLiteralBoolean: EventBoolean
    {
        private bool _boolean;

        private EventLiteralBoolean(bool boolean)
        {
            _boolean = boolean;
        }

        public static EventBoolean True()
        {
            return new EventLiteralBoolean(true);
        }

        public static EventBoolean False()
        {
            return new EventLiteralBoolean(false);
        }

        public override bool Evaluate(IEventContext context)
        {
            return _boolean;
        }
    }
}

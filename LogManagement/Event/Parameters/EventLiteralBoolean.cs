using System;

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

        public override string GetSyntax(IEventContext context)
        {
            return string.Format("[{0}]", _boolean.ToString());
        }
    }
}

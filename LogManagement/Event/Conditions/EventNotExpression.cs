﻿using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public class EventNotExpression : EventBoolean
    {
        private IEventBoolean _boolean;

        public EventNotExpression(IEventBoolean boolean)
        {
            _boolean = boolean;
        }

        public override bool Evaluate(IEventContext context)
        {
            return !_boolean.Evaluate(context);
        }

        public override string GetSyntax(IEventContext context)
        {
            string syntax = _boolean.GetSyntax(context);

            return string.Format("!({0})", syntax);
        }
    }
}

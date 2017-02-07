using System;
using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public class EventLessThanExpression : EventBoolean
    {
        private IEventData _data1;
        private IEventData _data2;

        public override bool Evaluate(IEventContext context)
        {
            IComparable comparable = (IComparable) _data1.GetData(context);

            return (comparable != null) && comparable.CompareTo(_data2.GetData(context)) < 0;
        }

        public EventLessThanExpression(IEventData data1, IEventData data2)
        {
            _data1 = data1;
            _data2 = data2;
        }

        public override string GetSyntax(IEventContext context)
        {
            string syntax1 = _data1.GetSyntax(context);
            string syntax2 = _data2.GetSyntax(context);

            return string.Format("({0} {1} {2})", syntax1, "<", syntax2);
        }
    }
}

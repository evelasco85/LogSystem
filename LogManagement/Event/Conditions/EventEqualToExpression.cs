using System;
using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public class EventEqualToExpression : EventBoolean
    {
        private IEventData _data1;
        private IEventData _data2;

        public override bool Evaluate(IEventContext context)
        {
            IComparable comparable = (IComparable)_data1.GetData(context);

            return (comparable != null) && comparable.CompareTo(_data2.GetData(context)) == 0;
        }

        public EventEqualToExpression(IEventData data1, IEventData data2)
        {
            _data1 = data1;
            _data2 = data2;
        }
    }
}

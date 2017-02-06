using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public interface IEventEqualExpression
    {
    }

    public class EventEqualExpression : EventBoolean, IEventEqualExpression
    {
        private IEventData _data1;
        private IEventData _data2;

        public override bool Evaluate(IEventContext context)
        {
            return _data1.GetData(context) == _data2.GetData(context);
        }

        public EventEqualExpression(IEventData data1, IEventData data2)
        {
            _data1 = data1;
            _data2 = data2;
        }
    }
}

namespace LogManagement.Event.Parameters
{
    public class EventLiteral : EventData
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

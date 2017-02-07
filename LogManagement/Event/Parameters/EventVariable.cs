namespace LogManagement.Event.Parameters
{
    public interface IEventVariable : IEventData
    {
        string Name { get; }
        object Value { get; set; }
    }

    public class EventVariable : EventData, IEventVariable
    {
        string _name;

        public string Name
        {
            get { return _name; }
        }

        public object Value { get; set; }

        public EventVariable(string name)
        {
            _name = name;
        }

        public override object GetData(IEventContext context)
        {
            IEventVariable variable = context.GetVariable(_name);

            return variable.Value;
        }
    }
}

using LogManagement.Event.Conditions;

namespace LogManagement.Event.Parameters
{
    public interface IEventVariable
    {
        string Name { get; }
        object Value { get; set; }
    }

    public class EventVariable : EventBoolean, IEventVariable
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

        public override bool Evaluate(IEventContext context)
        {
            //IEventVariable variable = context.GetVariable(_name);

            return true;
        }
    }
}

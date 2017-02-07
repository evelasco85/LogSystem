using System;

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
            Value = context.GetVariable(_name).Value;

            return Value;
        }

        public override string GetSyntax(IEventContext context)
        {
            return string.Format("[{0} : {1}]", _name, Convert.ToString(GetData(context)));
        }
    }
}

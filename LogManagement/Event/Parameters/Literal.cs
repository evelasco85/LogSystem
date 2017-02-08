using System;

namespace LogManagement.Event.Parameters
{
    public class Literal : Data
    {
        private object _value;
        private string _name;

        public Literal(string literalName, object value)
        {
            _value = value;
            _name = literalName;
        }

        public override string Name
        {
            get { return _name; }
        }

        public Literal(object value) : this("Value", value)
        {
        }

        public override object GetData(IContext context)
        {
            return _value;
        }

        public override string GetSyntax(IContext context)
        {
            return string.Format("[{0} : {1}]", _name, Convert.ToString(GetData(context)));
        }
    }
}

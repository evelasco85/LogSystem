using System;
using LogManagement.Dynamic.Event;

namespace LogManagement.Dynamic.Event.Parameters
{
    public class Variable : Data
    {
        string _name;

        public override string Name
        {
            get { return _name; }
        }

        public Variable(string name)
        {
            _name = name;
        }

        public override object GetData(IContext context)
        {
            return context.GetValue(_name);
        }

        public override string GetSyntax(IContext context)
        {
            return string.Format("[{0} : {1}]", _name, Convert.ToString(GetData(context)));
        }
    }
}

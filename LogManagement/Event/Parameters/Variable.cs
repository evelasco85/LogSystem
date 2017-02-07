using System;

namespace LogManagement.Event.Parameters
{
    public interface IVariable : IData
    {
        string Name { get; }
    }

    public class Variable : Data, IVariable
    {
        string _name;

        public string Name
        {
            get { return _name; }
        }

        public Variable(string name)
        {
            _name = name;
        }

        public override object GetData(IContext context)
        {
            return context.GetVariable(_name);
        }

        public override string GetSyntax(IContext context)
        {
            return string.Format("[{0} : {1}]", _name, Convert.ToString(GetData(context)));
        }
    }
}

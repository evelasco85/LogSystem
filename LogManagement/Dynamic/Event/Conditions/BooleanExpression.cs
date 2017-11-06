using LogManagement.Dynamic.Event;
using LogManagement.Dynamic.Event.Parameters;

namespace LogManagement.Dynamic.Event.Conditions
{
    public class BooleanExpression: BooleanBase
    {
        private bool _boolean;

        private BooleanExpression(bool boolean)
        {
            _boolean = boolean;
        }

        public static BooleanBase True()
        {
            return new BooleanExpression(true);
        }

        public static BooleanBase False()
        {
            return new BooleanExpression(false);
        }

        public override bool Evaluate(IContext context)
        {
            return _boolean;
        }

        public static string Operator
        {
            get { return string.Empty; }
        }

        public override string GetSyntax(IContext context)
        {
            return string.Format("[{0}]", _boolean.ToString());
        }
    }
}

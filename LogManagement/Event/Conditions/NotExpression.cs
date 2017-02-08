using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public class NotExpression : BooleanBase
    {
        private IBooleanBase _boolean;

        public static IBooleanBase New(IBooleanBase boolean)
        {
            return new NotExpression(boolean);
        }

        private NotExpression(IBooleanBase boolean)
        {
            _boolean = boolean;
        }

        public override bool Evaluate(IContext context)
        {
            return !_boolean.Evaluate(context);
        }

        public override string GetSyntax(IContext context)
        {
            string syntax = _boolean.GetSyntax(context);

            return string.Format("{0}({1})", Operator, syntax);
        }

        public static string Operator
        {
            get { return "!"; }
        }
    }
}

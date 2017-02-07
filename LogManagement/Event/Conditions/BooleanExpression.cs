using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
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

        public override string GetSyntax(IContext context)
        {
            return string.Format("[{0}]", _boolean.ToString());
        }
    }
}

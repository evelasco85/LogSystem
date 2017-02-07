using System;

namespace LogManagement.Event.Parameters
{
    public class LiteralBoolean: BooleanBase
    {
        private bool _boolean;

        private LiteralBoolean(bool boolean)
        {
            _boolean = boolean;
        }

        public static BooleanBase True()
        {
            return new LiteralBoolean(true);
        }

        public static BooleanBase False()
        {
            return new LiteralBoolean(false);
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

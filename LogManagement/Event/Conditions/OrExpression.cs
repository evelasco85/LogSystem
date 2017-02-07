using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public class OrExpression : BooleanBase
    {
        private IEventBoolean _operand1;
        private IEventBoolean _operand2;

        public override bool Evaluate(IContext context)
        {
            return _operand1.Evaluate(context) || _operand2.Evaluate(context);
        }

        public OrExpression(IEventBoolean operand1, IEventBoolean operand2)
        {
            _operand1 = operand1;
            _operand2 = operand2;
        }

        public override string GetSyntax(IContext context)
        {
            string syntax1 = _operand1.GetSyntax(context);
            string syntax2 = _operand2.GetSyntax(context);

            return string.Format("({0} {1} {2})", syntax1, "||", syntax2);
        }
    }
}

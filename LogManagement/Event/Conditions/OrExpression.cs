using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public class OrExpression : BooleanBase
    {
        private IBooleanBase _operand1;
        private IBooleanBase _operand2;

        public override bool Evaluate(IContext context)
        {
            return _operand1.Evaluate(context) || _operand2.Evaluate(context);
        }

        public OrExpression(IBooleanBase operand1, IBooleanBase operand2)
        {
            _operand1 = operand1;
            _operand2 = operand2;
        }

        public override string Operator
        {
            get { return "||"; }
        }

        public override string GetSyntax(IContext context)
        {
            string syntax1 = _operand1.GetSyntax(context);
            string syntax2 = _operand2.GetSyntax(context);

            return string.Format("({0} {1} {2})", syntax1, Operator, syntax2);
        }
    }
}

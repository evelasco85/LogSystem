using System;
using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public class LessThanExpression : BooleanBase
    {
        private IData _operand1;
        private IData _operand2;

        public static IBooleanBase New(IData operand1, IData operand2)
        {
            return new LessThanExpression(operand1, operand2);
        }

        private LessThanExpression(IData operand1, IData operand2)
        {
            _operand1 = operand1;
            _operand2 = operand2;
        }

        public override bool Evaluate(IContext context)
        {
            IComparable comparable = (IComparable)_operand1.GetData(context);

            return (comparable != null) && comparable.CompareTo(_operand2.GetData(context)) < 0;
        }

        public override string Operator
        {
            get { return "<"; }
        }

        public override string GetSyntax(IContext context)
        {
            string syntax1 = _operand1.GetSyntax(context);
            string syntax2 = _operand2.GetSyntax(context);

            return string.Format("({0} {1} {2})", syntax1, Operator, syntax2);
        }
    }
}

using System.Collections.Generic;
using System.Linq;
using LogManagement.Event.Parameters;

namespace LogManagement.Event.Conditions
{
    public class AndExpression : BooleanBase
    {
        private List<IBooleanBase> _operands;

        public static IBooleanBase New(params IBooleanBase[] operands)
        {
            return new AndExpression(operands);
        }

        public AndExpression(params IBooleanBase[] operands)
        {
            _operands = operands.ToList();
        }

        public override bool Evaluate(IContext context)
        {
            bool isTrue = true;

            _operands.ForEach(operand =>
            {
                isTrue = isTrue && operand.Evaluate(context);

                if (!isTrue)
                    return;
            });

            return isTrue;
        }

        public static string Operator
        {
            get { return "&&"; }
        }

        public override string GetSyntax(IContext context)
        {
            string syntax = string.Join(
                string.Format(" {0} ", Operator),
                _operands
                    .Select(x => x.GetSyntax(context)));

            return syntax;
        }
    }
}

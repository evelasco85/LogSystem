using System;
using System.Collections.Generic;
using LogManagement.Event.Parameters;

namespace LogManagement.Event
{
    public interface IRuleValidation
    {
        void Validate(IContext context, SuccessfulConditionsInvokedDelegate successfulResultInvocation,
           FailedConditionsInvokedDelegate failedResultInvocation);
    }

    public interface IRule : IRuleValidation
    {
        IRule RegisterVariable(IVariable variable);
        IRuleValidation RegisterCondition(IBooleanBase condition);
    }

    public delegate void SuccessfulConditionsInvokedDelegate();
    public delegate void FailedConditionsInvokedDelegate();

    public class Rule : IRule
    {
        IDictionary<string, IVariable> _variables = new Dictionary<string, IVariable>();
        private IBooleanBase _condition;

        public IRule RegisterVariable(IVariable variable)
        {
            if (variable == null)
                throw new ArgumentException("'variable' parameter is required");

            if(_variables.ContainsKey(variable.Name))
                throw new ArgumentException(string.Format("Variable with name '{0}' is already registered", variable.Name));

            _variables.Add(variable.Name, variable);

            return this;
        }

        public IRuleValidation RegisterCondition(IBooleanBase condition)
        {
            _condition = condition;

            return this;
        }

        public void Validate(IContext context, SuccessfulConditionsInvokedDelegate successfulResultInvocation, FailedConditionsInvokedDelegate failedResultInvocation)
        {
            if(context == null)
                return;

            bool success = _condition.Evaluate(context);

            if ((success) && (successfulResultInvocation != null))
                    successfulResultInvocation();
            else
            {
                if (failedResultInvocation != null)
                    failedResultInvocation();
            }
        }
    }
}

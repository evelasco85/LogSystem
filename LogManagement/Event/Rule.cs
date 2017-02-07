using System;
using System.Collections.Generic;
using LogManagement.Event.Parameters;

namespace LogManagement.Event
{
    public interface IRule
    {
        IRule RegisterVariable(IVariable variable);
        IRule RegisterCondition(IEventBoolean condition);

        void Validate(IContext context, SuccessfulConditionsInvokedDelegate successfulResultInvocation,
            FailedConditionsInvokedDelegate failedResultInvocation);
    }

    public delegate void SuccessfulConditionsInvokedDelegate(IList<IEventBoolean> successfulConditions);
    public delegate void FailedConditionsInvokedDelegate(IList<IEventBoolean> failedConditions);

    public class Rule : IRule
    {
        IDictionary<string, IVariable> _variables = new Dictionary<string, IVariable>();
        IList<IEventBoolean> _conditions = new List<IEventBoolean>();

        public IRule RegisterVariable(IVariable variable)
        {
            if (variable == null)
                throw new ArgumentException("'variable' parameter is required");

            if(_variables.ContainsKey(variable.Name))
                throw new ArgumentException(string.Format("Variable with name '{0}' is already registered", variable.Name));

            _variables.Add(variable.Name, variable);

            return this;
        }

        public IRule RegisterCondition(IEventBoolean condition)
        {
            _conditions.Add(condition);

            return this;
        }

        public void Validate(IContext context, SuccessfulConditionsInvokedDelegate successfulResultInvocation, FailedConditionsInvokedDelegate failedResultInvocation)
        {
            if(context == null)
                return;

            IList<IEventBoolean> successfulConditions = new List<IEventBoolean>();
            IList<IEventBoolean> failedConditions = new List<IEventBoolean>();

            for (int index = 0; index < _conditions.Count; index++)
            {
                IEventBoolean condition = _conditions[index];
                bool success = condition.Evaluate(context);

                if(success)
                    successfulConditions.Add(condition);
                else
                    failedConditions.Add(condition);
            }

            if (successfulResultInvocation != null)
                successfulResultInvocation(successfulConditions);

            if (failedResultInvocation != null)
                failedResultInvocation(failedConditions);
        }
    }
}

using System;
using System.Collections.Generic;
using LogManagement.Event.Parameters;

namespace LogManagement.Event
{
    public interface IEventRule
    {
        IEventRule RegisterContextValue(string variableName, object value);
        IEventRule RegisterVariable(IEventVariable variable);
        IEventRule RegisterCondition(IEventBoolean condition);

        void Validate(SuccessfulConditionsInvokedDelegate successfulResultInvocation,
            FailedConditionsInvokedDelegate failedResultInvocation);
    }

    public delegate void SuccessfulConditionsInvokedDelegate(IList<IEventBoolean> successfulConditions);
    public delegate void FailedConditionsInvokedDelegate(IList<IEventBoolean> failedConditions);

    public class EventRule : IEventRule
    {
        IEventContext _context = new EventContext();
        IDictionary<string, IEventVariable> _variables = new Dictionary<string, IEventVariable>();
        IList<IEventBoolean> _conditions = new List<IEventBoolean>();

        public IEventRule RegisterVariable(IEventVariable variable)
        {
            if (variable == null)
                throw new ArgumentException("'variable' parameter is required");

            if(_variables.ContainsKey(variable.Name))
                throw new ArgumentException(string.Format("Variable with name '{0}' is already registered", variable.Name));

            _variables.Add(variable.Name, variable);
            _context.Assign(variable, null);

            return this;
        }

        public IEventRule RegisterCondition(IEventBoolean condition)
        {
            _conditions.Add(condition);

            return this;
        }

        public void Validate(SuccessfulConditionsInvokedDelegate successfulResultInvocation, FailedConditionsInvokedDelegate failedResultInvocation)
        {
            IList<IEventBoolean> successfulConditions = new List<IEventBoolean>();
            IList<IEventBoolean> failedConditions = new List<IEventBoolean>();

            for (int index = 0; index < _conditions.Count; index++)
            {
                IEventBoolean condition = _conditions[index];
                bool success = condition.Evaluate(_context);

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

        public IEventRule RegisterContextValue(string variableName, object value)
        {
            _context.Assign(variableName, value);

            return this;
        }
    }
}

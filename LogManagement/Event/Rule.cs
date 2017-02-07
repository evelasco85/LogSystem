using System;
using System.Collections.Generic;
using System.Linq;
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
        string Id { get; }
        IRule RegisterVariable(IVariable variable);
        IRule RegisterVariable(IVariable variable, bool requiredForInvocation);
        IRuleValidation RegisterCondition(IBooleanBase condition);
        bool CanInvoke(IContext context);
    }

    public delegate void SuccessfulConditionsInvokedDelegate();
    public delegate void FailedConditionsInvokedDelegate();

    public class Rule : IRule
    {
        private string _id;
        IDictionary<string, IVariable> _variables = new Dictionary<string, IVariable>();
        IDictionary<string, bool> _requiredVariables = new Dictionary<string, bool>();
        private IBooleanBase _condition;

        public string Id
        {
            get { return _id; }
        }

        public Rule(string id)
        {
            _id = id;
        }

        public IRule RegisterVariable(IVariable variable)
        {
            RegisterVariable(variable, false);

            return this;
        }

        public IRule RegisterVariable(IVariable variable, bool requiredForInvocation)
        {
            if (variable == null)
                throw new ArgumentException("'variable' parameter is required");

            if(_variables.ContainsKey(variable.Name))
                throw new ArgumentException(string.Format("Variable with name '{0}' is already registered", variable.Name));

            _variables.Add(variable.Name, variable);
            _requiredVariables.Add(variable.Name, requiredForInvocation);

            return this;
        }

        public IRuleValidation RegisterCondition(IBooleanBase condition)
        {
            _condition = condition;

            return this;
        }

        public bool CanInvoke(IContext context)
        {
            IEnumerable<string> contextVariableNames = context.GetVariableNameList();
            IEnumerable<string> requiredVariables = _requiredVariables
                .Where(kvp => kvp.Value == true)
                .Select(kvp => kvp.Key);

            return !requiredVariables.Except(contextVariableNames).Any();
        }

        public void Validate(IContext context, SuccessfulConditionsInvokedDelegate successfulResultInvocation, FailedConditionsInvokedDelegate failedResultInvocation)
        {
            if (context == null)
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

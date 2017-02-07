using System;
using System.Collections.Generic;
using System.Linq;
using LogManagement.Event.Parameters;

namespace LogManagement.Event
{
    public interface IRuleValidation
    {
        void Validate(IContext context);
    }

    public interface IRule : IRuleValidation
    {
        string Id { get; }
        SuccessfulConditionsInvokedDelegate SuccessfulResultInvocation { get; set; }
        FailedConditionsInvokedDelegate FailedResultInvocation { get; set; }
        IList<IVariable> GetVariables();
        IRule AddVariableScope(IVariable variable);
        IRule AddVariableScope(IVariable variable, bool requiredForInvocation);
        IRuleValidation SetCondition(IBooleanBase condition);
        IRuleValidation SetCondition(IBooleanBase condition,
            SuccessfulConditionsInvokedDelegate successfulResultInvocation,
            FailedConditionsInvokedDelegate failedResultInvocation);
        IBooleanBase GetCondition();
        bool CanInvokeRule(IContext context);
    }

    public delegate void SuccessfulConditionsInvokedDelegate(IContext resultContext, IRule resultRule);
    public delegate void FailedConditionsInvokedDelegate(IContext resultContext, IRule resultRule);

    public class Rule : IRule
    {
        private string _id;
        IDictionary<string, IVariable> _variables = new Dictionary<string, IVariable>();
        IDictionary<string, bool> _requiredVariables = new Dictionary<string, bool>();
        private IBooleanBase _condition;

        public SuccessfulConditionsInvokedDelegate _successfulResultInvocation;
        public FailedConditionsInvokedDelegate _failedResultInvocation;

        public string Id
        {
            get { return _id; }
        }

        public SuccessfulConditionsInvokedDelegate SuccessfulResultInvocation
        {
            get { return _successfulResultInvocation; }
            set { _successfulResultInvocation = value; }
        }

        public FailedConditionsInvokedDelegate FailedResultInvocation
        {
            get { return _failedResultInvocation; }
            set { _failedResultInvocation = value; }
        }

        public Rule(string id)
        {
            _id = id;
        }

        public IRule AddVariableScope(IVariable variable)
        {
            AddVariableScope(variable, false);

            return this;
        }

        public IRule AddVariableScope(IVariable variable, bool requiredForInvocation)
        {
            if (variable == null)
                throw new ArgumentException("'variable' parameter is required");

            if(_variables.ContainsKey(variable.Name))
                throw new ArgumentException(string.Format("Variable with name '{0}' is already registered", variable.Name));

            _variables.Add(variable.Name, variable);
            _requiredVariables.Add(variable.Name, requiredForInvocation);

            return this;
        }

        public IRuleValidation SetCondition(IBooleanBase condition, 
            SuccessfulConditionsInvokedDelegate successfulResultInvocation, 
            FailedConditionsInvokedDelegate failedResultInvocation)
        {
            _condition = condition;
            _successfulResultInvocation = successfulResultInvocation;
            _failedResultInvocation = failedResultInvocation;

            return this;
        }

        public IBooleanBase GetCondition()
        {
            return _condition;
        }

        public IRuleValidation SetCondition(IBooleanBase condition)
        {
            SetCondition(condition, null, null);
            return this;
        }

        public IList<IVariable> GetVariables()
        {
            return _variables
                .Select(kvp => kvp.Value)
                .ToList();
        }

        public bool CanInvokeRule(IContext context)
        {
            IEnumerable<string> contextVariableNames = context.GetVariableNameList();
            IEnumerable<string> requiredVariables = _requiredVariables
                .Where(kvp => kvp.Value == true)
                .Select(kvp => kvp.Key);

            return !requiredVariables.Except(contextVariableNames).Any();
        }

        public void Validate(IContext context)
        {
            if (context == null)
                return;

            bool success = _condition.Evaluate(context);

            if ((success) && (_successfulResultInvocation != null))
                _successfulResultInvocation(context, this);
            else
            {
                if (_failedResultInvocation != null)
                    _failedResultInvocation(context, this);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using LogManagement.Event;

namespace LogManagement.Managers
{
    public interface IRuleManager
    {
        void AddRule(IRule rule);
        IList<IRule> GetMatchingRules(IContext context);
        void InvokeMatchingRules(IContext context,
            SuccessfulConditionsInvokedDelegate additionalSuccessfulResultInvocation,
            FailedConditionsInvokedDelegate additionalFailedResultInvocation);
    }

    public class RuleManager : IRuleManager
    {
        static IRuleManager s_instance = new RuleManager();
        IDictionary<string, IRule> _rules = new Dictionary<string, IRule>();

        private RuleManager()
        {
        }

        public static IRuleManager GetInstance()
        {
            return s_instance;
        }

        public void AddRule(IRule rule)
        {
            if(rule == null)
                throw new ArgumentException("'rule' parameter is required");

            if(_rules.ContainsKey(rule.Id))
                throw new ArgumentException(string.Format("Rule parameter with rule-id '{0}' already added", rule.Id));

            _rules.Add(rule.Id, rule);
        }

        public IList<IRule> GetMatchingRules(IContext context)
        {
            return _rules
                .Where(kvp => kvp.Value.CanInvokeRule(context))
                .Select(kvp => kvp.Value)
                .ToList();
        }

        public void InvokeMatchingRules(IContext context, 
            SuccessfulConditionsInvokedDelegate additionalSuccessfulResultInvocation,
            FailedConditionsInvokedDelegate additionalFailedResultInvocation)
        {
            List<IRule> matchingRules = (List<IRule>)GetMatchingRules(context);

            if(!matchingRules.Any())
                return;

            matchingRules.ForEach(rule =>
            {
                SuccessfulConditionsInvokedDelegate originalSuccessfulResultInvocation = rule.SuccessfulResultInvocation;
                FailedConditionsInvokedDelegate originalFailedResultInvocation = rule.FailedResultInvocation;

                rule.SuccessfulResultInvocation = () =>
                {
                    InvokeRuleDelegate(originalSuccessfulResultInvocation);
                    InvokeRuleDelegate(additionalSuccessfulResultInvocation);
                };

                rule.FailedResultInvocation = () =>
                {
                    InvokeRuleDelegate(originalFailedResultInvocation);
                    InvokeRuleDelegate(additionalFailedResultInvocation);
                };

                rule.Validate(context);

                //Revert to original delegates
                rule.SuccessfulResultInvocation = originalSuccessfulResultInvocation;
                rule.FailedResultInvocation = originalFailedResultInvocation;
            });
        }

        void InvokeRuleDelegate(SuccessfulConditionsInvokedDelegate successfulResultInvocation)
        {
            if (successfulResultInvocation != null)
                successfulResultInvocation();
        }
        void InvokeRuleDelegate(FailedConditionsInvokedDelegate failedResultInvocation)
        {
            if (failedResultInvocation != null)
                failedResultInvocation();
        }
    }
}

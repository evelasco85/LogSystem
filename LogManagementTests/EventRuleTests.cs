using System;
using System.Collections.Generic;
using LogManagement.Event;
using LogManagement.Event.Conditions;
using LogManagement.Event.Parameters;
using LogManagement.Managers;
using LogManagement.Registration;
using LogManagementTests.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class EventRuleTests
    {
        IVariable _isAdminVar = new Variable("Is Administrator");
        private IVariable _accessRightsVar = new Variable("Access Rights");
        IVariable _componentVar = new Variable("Component Name");
        IVariable _eventNameVar = new Variable("EventName");

        const string AUTHENTICATION_COMPONENT_NAME = "Authentication Component";
        const string AUTHENTICATION_EVENT_NAME = "Validation";
         

        void PerformRegistrations()
        {
            IApplicationRegistration applicationRegistration = new ApplicationRegistration("Security Tester");
            applicationRegistration
                .RegisterComponent(
                    (new ComponentRegistration<Authentication>(AUTHENTICATION_COMPONENT_NAME, applicationRegistration))
                        .RegisterObservableEvent(AUTHENTICATION_EVENT_NAME, authentication => new Func<bool>(authentication.Verify))
                        .RegisterObservableParameter(_isAdminVar.Name, authentication => authentication.AdministratorAccess)
                        .RegisterObservableParameter(_accessRightsVar.Name, authentication => authentication.AccessRights)
                )
                .RegisterComponent(
                    (new ComponentRegistration<SecurityCredential>("Security Credential Component", applicationRegistration))
                    .RegisterObservableEvent("Credential Setup", securityCredential => new Action<string, string>(securityCredential.SetCredentials))
                    .RegisterObservableEvent("Credential Input Validation", securityCredential => new Func<bool>(securityCredential.ValidateCredentialInput))

                );

            ISystemRegistration systemRegistration = new SystemRegistration("Security System");

            systemRegistration
                .RegisterApplication(applicationRegistration)
                .RegisterApplication(new ApplicationRegistration("Dummy Application"));


            ActivityManager.GetInstance().RegisterSystem(systemRegistration);
        }

        void SetupRules()
        {
            IRule accessRightsViolationRule = new Rule(Guid.NewGuid().ToString());

            accessRightsViolationRule
                .AddVariableScope(_componentVar)
                .AddVariableScope(_eventNameVar)
                .AddVariableScope(_isAdminVar, true)
                .AddVariableScope(_accessRightsVar, true);

            //Event-trigger condition
            IBooleanBase componentCondition = new EqualToExpression(_componentVar, new Literal(AUTHENTICATION_COMPONENT_NAME));
            IBooleanBase eventCondition = new EqualToExpression(_eventNameVar, new Literal(AUTHENTICATION_EVENT_NAME));
            IBooleanBase matchingEventCondition = new AndExpression(componentCondition, eventCondition);

            //Parameter-trigger condition
            IBooleanBase accessRightsCondition = new EqualToExpression(_accessRightsVar, new Literal(Rights.Full));
            IBooleanBase isAdministratorCondition = new EqualToExpression(_isAdminVar, new Literal(true));
            IBooleanBase notAllowedAccessRightsCondition = new AndExpression(accessRightsCondition, new NotExpression(isAdministratorCondition));

            //Trigger condition
            IBooleanBase triggerCondition = new AndExpression(matchingEventCondition, notAllowedAccessRightsCondition);

            accessRightsViolationRule.SetCondition(triggerCondition,
                () =>{
                    Console.WriteLine("Successfully invoked");
                },
                () =>{
                    Console.WriteLine("Failed to invoked");
                });

            RuleManager.GetInstance().AddRule(accessRightsViolationRule);
        }

        [TestInitialize]
        public void Initialize()
        {
            PerformRegistrations();
            SetupRules();
        }

        [TestMethod]
        public void TestRuleValidation()
        {
            string errorMessage = "Rule validation not invoked";

            ActivityManager.GetInstance().OnActivityEmit =
                (systemName, applicationName, componentName, eventName, parameters) =>
                {
                    IContext context = new Context();

                    context
                        .Assign(_componentVar.Name, componentName)
                        .Assign(_eventNameVar.Name, eventName);

                    ((List<Tuple<string, object>>) parameters).ForEach(parameter =>
                    {
                        context.Assign(parameter.Item1, parameter.Item2);
                    });

                    RuleManager.GetInstance().InvokeMatchingRules(context,
                        () => {
                                  errorMessage = "Non-administrator should have limited access rights!";
                        },
                        () => {
                                  errorMessage = "Rule validation was invoked but access-rights is a non-violation";
                        });
                };

            Authentication auth = new Authentication()
            {
                //Violative rights, non-administrator w/full access?
                AccessRights = Rights.Full,
                AdministratorAccess = false
            };

            bool verified = auth.Verify();      //This will emit activity detail

            Assert.IsFalse(verified);
            Assert.AreEqual("Non-administrator should have limited access rights!", errorMessage);
        }
    }
}

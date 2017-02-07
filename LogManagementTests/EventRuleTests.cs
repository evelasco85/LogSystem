using System;
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

        [TestInitialize]
        public void Initialize()
        {
            IApplicationRegistration applicationRegistration = new ApplicationRegistration("Security Tester");
            applicationRegistration
                .RegisterComponent(
                    (new ComponentRegistration<Authentication>("Authentication Component", applicationRegistration))
                        .RegisterObservableEvent("Validation", authentication => new Func<bool>(authentication.Verify))
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

        [TestMethod]
        public void TestRuleValidation()
        {
            IVariable componentVar = new Variable("Component Name");
            IVariable eventNameVar = new Variable("EventName");

            IRule accessRightsViolationRule = new Rule(Guid.NewGuid().ToString());

            accessRightsViolationRule
                .RegisterVariable(componentVar)
                .RegisterVariable(eventNameVar)
                .RegisterVariable(_isAdminVar, true)
                .RegisterVariable(_accessRightsVar, true);

            //Event-trigger condition
            IBooleanBase componentCondition = new EqualToExpression(componentVar, new Literal("Authentication Component"));
            IBooleanBase eventCondition = new EqualToExpression(eventNameVar, new Literal("Validation"));
            IBooleanBase matchingEventCondition = new AndExpression(componentCondition, eventCondition);

            //Parameter-trigger condition
            IBooleanBase accessRightsCondition = new EqualToExpression(_accessRightsVar, new Literal(Rights.Full));
            IBooleanBase isAdministratorCondition = new EqualToExpression(_isAdminVar, new Literal(true));
            IBooleanBase notAllowedAccessRightsCondition = new AndExpression(accessRightsCondition, new NotExpression(isAdministratorCondition));

            //Trigger condition
            IBooleanBase triggerCondition = new AndExpression(matchingEventCondition, notAllowedAccessRightsCondition);

            accessRightsViolationRule.RegisterCondition(triggerCondition);

            string errorMessage = "Rule validation not invoked";

            ActivityManager.GetInstance().OnActivityEmit =
                (systemName, applicationName, componentName, eventName, parameters) =>
                {
                    IContext context = new Context();

                    context
                        .Assign("Component Name", componentName)
                        .Assign("EventName", eventName);

                    for (int index = 0; index < parameters.Count; index++)
                    {
                        Tuple<string, object> parameter = parameters[index];

                        context.Assign(parameter.Item1, parameter.Item2);
                    }

                    accessRightsViolationRule.Validate(context, () =>
                    {
                        errorMessage = "Non-administrator should have limited access rights!";
                    },
                        () =>
                        {
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

            Assert.AreEqual("Non-administrator should have limited access rights!", errorMessage);
        }
    }
}

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
        [TestInitialize]
        public void Initialize()
        {
            IApplicationRegistration applicationRegistration = new ApplicationRegistration("Security Tester");
            applicationRegistration
                .RegisterComponent(
                    (new ComponentRegistration<Authentication>("Authentication Component", applicationRegistration))
                        .RegisterObservableEvent("Validation", authentication => new Func<bool>(authentication.Verify))
                        .RegisterObservableParameter("Is Administrator", authentication => authentication.AdministratorAccess)
                        .RegisterObservableParameter("Access Rights", authentication => authentication.AccessRights)
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
            IVariable compVar = new Variable("Component Name");
            IVariable evNameVar = new Variable("EventName");
            IVariable isAdminVar = new Variable("Is Administrator");
            IVariable accessRightsVar = new Variable("Access Rights");

            IBooleanBase accessRightsCondition = new EqualToExpression(accessRightsVar, new Literal(Rights.Full));
            IBooleanBase isAdministratorCondition = new EqualToExpression(isAdminVar, new Literal(true));
            IBooleanBase notAllowedAccessRightsCondition = new AndExpression(accessRightsCondition, new NotExpression(isAdministratorCondition));

            IRule accessRightsViolationRule = new Rule();

            accessRightsViolationRule
                .RegisterVariable(compVar)
                .RegisterVariable(evNameVar)
                .RegisterVariable(isAdminVar, true)
                .RegisterVariable(accessRightsVar, true)
                ;

            accessRightsViolationRule.RegisterCondition(notAllowedAccessRightsCondition);

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

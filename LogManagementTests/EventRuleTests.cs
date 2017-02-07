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
        public void TestMethod1()
        {
            IEventVariable compVar = new EventVariable("Component Name");
            IEventVariable evNameVar = new EventVariable("EventName");
            IEventVariable isAdminVar = new EventVariable("Is Administrator");
            IEventVariable accessRightsVar = new EventVariable("Access Rights");

            IEventBoolean compCondition = new EqualToExpression(compVar, new EventLiteral("Authentication Component"));
            IEventBoolean evNameCondition = new EqualToExpression(evNameVar, new EventLiteral("Validation"));
            IEventBoolean compEventCondition = new AndExpression(compCondition, evNameCondition);
            IEventBoolean accessRightsCondition = new EqualToExpression(accessRightsVar, new EventLiteral(Rights.Full));
            IEventBoolean isAdministratorCondition = new EqualToExpression(isAdminVar, new EventLiteral(true));
            IEventBoolean notAllowedAccessRightsCondition = new AndExpression(accessRightsCondition, new NotExpression(isAdministratorCondition));
            IEventBoolean condition = new AndExpression(compEventCondition, notAllowedAccessRightsCondition);

            IEventRule accessRightsViolationRule = new EventRule();

            accessRightsViolationRule
                .RegisterVariable(compVar)
                .RegisterVariable(evNameVar)
                .RegisterVariable(isAdminVar)
                .RegisterVariable(accessRightsVar)
                ;

            accessRightsViolationRule.RegisterCondition(condition);

            string errorMessage = string.Empty;

            ActivityManager.GetInstance().OnActivityEmit =
                (systemName, applicationName, componentName, eventName, parameters) =>
                {
                    IEventContext context = new EventContext();

                    context
                        .Assign("Component Name", componentName)
                        .Assign("EventName", eventName);

                    for (int index = 0; index < parameters.Count; index++)
                    {
                        Tuple<string, object> parameter = parameters[index];

                        context.Assign(parameter.Item1, parameter.Item2);
                    }

                    accessRightsViolationRule.Validate(context, successfulConditions =>
                    {
                        errorMessage = "Non-administrator should have limited access rights!";
                    },
                        failingConditions =>
                        {
                            
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

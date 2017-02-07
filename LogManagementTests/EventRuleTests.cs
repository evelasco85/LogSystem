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

            IEventBoolean compCondition = new EventEqualToExpression(compVar, new EventLiteral("Authentication Component"));
            IEventBoolean evNameCondition = new EventEqualToExpression(evNameVar, new EventLiteral("Validation"));
            IEventBoolean compEventCondition = new EventAndExpression(compCondition, evNameCondition);
            IEventBoolean accessRightsCondition = new EventEqualToExpression(accessRightsVar, new EventLiteral(Rights.Full));
            IEventBoolean isAdministratorCondition = new EventEqualToExpression(isAdminVar, new EventLiteral(true));
            IEventBoolean notAllowedAccessRightsCondition = new EventAndExpression(accessRightsCondition, new EventNotExpression(isAdministratorCondition));
            IEventBoolean condition = new EventAndExpression(compEventCondition, notAllowedAccessRightsCondition);

            IEventRule rule = new EventRule();

            rule
                .RegisterVariable(compVar)
                .RegisterVariable(evNameVar)
                .RegisterVariable(isAdminVar)
                .RegisterVariable(accessRightsVar)
                ;

            rule.RegisterCondition(condition);

            string errorMessage = string.Empty;

            ActivityManager.GetInstance().OnActivityEmit =
                (systemName, applicationName, componentName, eventName, parameters) =>
                {
                    rule
                        .RegisterContextValue("Component Name", componentName)
                        .RegisterContextValue("EventName", eventName);

                    for (int index = 0; index < parameters.Count; index++)
                    {
                        Tuple<string, object> parameter = parameters[index];

                        rule.RegisterContextValue(parameter.Item1, parameter.Item2);
                    }
                    

                    rule.Validate(successfulConditions =>
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

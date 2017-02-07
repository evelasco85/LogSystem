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
            IList<Tuple<string, object>> evParams = null;

            IEventVariable sysVar = new EventVariable("System Name");
            IEventVariable appVar = new EventVariable("Application Name");
            IEventVariable compVar = new EventVariable("Component Name");
            IEventVariable evNameVar = new EventVariable("EventName");

            IEventBoolean sysCondition = new EventEqualToExpression(sysVar, new EventLiteral("Security System"));
            IEventBoolean appCondition = new EventEqualToExpression(appVar, new EventLiteral("Security Testers"));
            IEventBoolean compCondition = new EventEqualToExpression(compVar, new EventLiteral("Authentication Component"));
            IEventBoolean evNameCondition = new EventEqualToExpression(evNameVar, new EventLiteral("Validation"));
            
            IEventRule rule = new EventRule();

            rule
                .RegisterVariable(sysVar)
                .RegisterVariable(appVar)
                .RegisterVariable(compVar)
                .RegisterVariable(evNameVar);

            rule
                .RegisterCondition(sysCondition)
                .RegisterCondition(appCondition)
                .RegisterCondition(compCondition)
                .RegisterCondition(evNameCondition);

            ActivityManager.GetInstance().OnActivityEmit =
                (systemName, applicationName, componentName, eventName, parameters) =>
                {
                    rule
                        .RegisterContextValue("System Name", systemName)
                        .RegisterContextValue("Application Name", applicationName)
                        .RegisterContextValue("Component Name", componentName)
                        .RegisterContextValue("EventName", eventName);

                    rule.Validate(successfulConditions =>
                    {
                        
                    },
                        failingConditions =>
                        {
                            
                        });

                    evParams = parameters;
                };

            Authentication auth = new Authentication()
            {
                //Violative rights, non-administrator w/full access?
                AccessRights = Rights.Full,
                AdministratorAccess = false
            };

            bool verified = auth.Verify();      //This will emit activity detail

        }
    }
}

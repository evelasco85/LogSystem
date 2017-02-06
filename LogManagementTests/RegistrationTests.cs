using System;
using System.Collections.Generic;
using System.Linq;
using LogManagement.Managers;
using LogManagement.Registration;
using LogManagementTests.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class RegistrationTests
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
        public void TestValidateCurrentCall()
        {
            string sysName = string.Empty;
            string appName = string.Empty;
            string compName = string.Empty;
            string evName = string.Empty;
            IList<Tuple<string, object>> evParams = null;

            ActivityManager.GetInstance().OnActivityEmit =
                (systemName, applicationName, componentName, eventName, parameters) =>
                {
                    sysName = systemName;
                    appName = applicationName;
                    compName = componentName;
                    evName = eventName;
                    evParams = parameters;
                };

            Authentication auth = new Authentication()
            {
                //Violative rights, non-administrator w/full access?
                AccessRights = Rights.Full,
                AdministratorAccess = false
            };

            bool verified = auth.Verify();      //This will emit activity detail

            Assert.IsFalse(verified);
            Assert.AreEqual("Security System", sysName);
            Assert.AreEqual("Security Tester", appName);
            Assert.AreEqual("Authentication Component", compName);
            Assert.AreEqual("Validation", evName);
            Assert.IsTrue((evParams != null) && (evParams.Count == 2));

            Assert.AreEqual(Rights.Full, evParams.Where(p => p.Item1 == "Access Rights").First().Item2);
            Assert.AreEqual(false, evParams.Where(p => p.Item1 == "Is Administrator").First().Item2);
        }
    }
}

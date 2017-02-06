using System;
using LogManagement;
using LogManagement.Managers;
using LogManagement.Registration;
using LogManagementTests.Implementations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LogManagementTests
{
    [TestClass]
    public class RegistrationTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            IApplicationRegistration applicationRegistration = new ApplicationRegistration("Security Tester");
            applicationRegistration
                .RegisterComponent(
                    (new ComponentRegistration<Authentication>("Authentication Component"))
                        .RegisterObservableEvent("Validation", authentication => new Func<bool>(authentication.Verify))
                        .RegisterObservableParameter("Is Administrator", authentication => authentication.AdministratorAccess)
                        .RegisterObservableParameter("Access Rights", authentication => authentication.AccessRights)
                )
                .RegisterComponent(
                    (new ComponentRegistration<SecurityCredential>("Security Credential Component"))
                    .RegisterObservableEvent("Credential Setup", securityCredential => new Action<string, string>(securityCredential.SetCredentials))
                    .RegisterObservableEvent("Credential Input Validation", securityCredential => new Func<bool>(securityCredential.ValidateCredentialInput))

                );

            ISystemRegistration systemRegistration = new SystemRegistration("Security System");

            systemRegistration
                .RegisterApplication(applicationRegistration)
                .RegisterApplication(new ApplicationRegistration("Dummy Application"));


            ActivityManager.GetInstance().RegisterSystem(systemRegistration);

            Authentication auth = new Authentication()
            {
                //Violative rights, non-administrator w/full access?
                AccessRights = Rights.Full,
                AdministratorAccess = false
            };

            bool verified = auth.Verify();
        }
    }
}

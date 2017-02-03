using System;
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
                        .RegisterEvent("Validation", authentication => new Func<bool>(authentication.Verify))
                        .RegisterPrimitiveProperty("Is Administrator", authentication => authentication.AdministratorAccess)
                        .RegisterPrimitiveProperty("Access Rights", authentication => authentication.AccessRights)
                )
                .RegisterComponent(
                    (new ComponentRegistration<SecurityCredential>("Security Credential Component"))
                    .RegisterEvent("Credential Setup", securityCredential => new Action<string, string>(securityCredential.SetCredentials))
                    .RegisterEvent("Credential Input Validation", securityCredential => new Func<bool>(securityCredential.ValidateCredentialInput))

                );

            ISystemRegistration systemRegistration = new SystemRegistration("Security System");

            systemRegistration
                .RegisterApplication(applicationRegistration)
                .RegisterApplication(new ApplicationRegistration("Dummy Application"));
        }
    }
}

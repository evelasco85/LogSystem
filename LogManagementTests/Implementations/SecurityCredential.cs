using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogManagementTests.Implementations
{
    public class SecurityCredential
    {
        public string Username { get; set; }
        public string Password { get; set; }

        public void SetCredentials(string username, string password)
        {
            Username = username;
            Password = password;
        }

        public bool ValidateCredentialInput()
        {
            string badPassword = "@dm1n";
            bool isValid = !Password.Contains(badPassword);

            return isValid;
        }
    }
}

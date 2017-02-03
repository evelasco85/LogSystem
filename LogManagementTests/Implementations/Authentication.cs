using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LogManagement;

namespace LogManagementTests.Implementations
{
    public enum Rights
    {
        Full,
        ReadOnly
    }

    public class Authentication
    {
        public Rights AccessRights { get; set; }
        public bool AdministratorAccess { get; set; }

        public bool Verify()
        {
            return false;
        }

        public Authentication()
        {
            //Test-only: Example of anomalous rights
            AccessRights = Rights.Full;
            AdministratorAccess = false;
        }
    }
}

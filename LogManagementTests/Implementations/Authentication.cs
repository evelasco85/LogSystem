using LogManagement.Managers;

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
            bool violative = (AdministratorAccess == false) && (AccessRights == Rights.Full);

            ActivityManager.GetInstance().ValidateCurrentCall(this);

            return !violative;
        }

        public Authentication()
        {
        }
    }
}

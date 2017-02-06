using System.Runtime.CompilerServices;
using LogManagement.Registration;

namespace LogManagement.Managers
{
    public interface IActivityManager
    {
        void RegisterSystem(ISystemRegistration systemRegistration);
        void ValidateCurrentCall<TCallingInstance>(TCallingInstance instance, [CallerMemberName] string memberName = "");
    }

    public class ActivityManager : IActivityManager
    {
        static IActivityManager s_instance = new ActivityManager();
        IActivityMonitoring _monitoring  = new ActivityMonitoring();
        private ISystemRegistration _systemRegistration;

        private ActivityManager()
        {
        }

        public static IActivityManager GetInstance()
        {
            return s_instance;
        }

        public void RegisterSystem(ISystemRegistration systemRegistration)
        {
            _systemRegistration = systemRegistration;
        }

        public void ValidateCurrentCall<TCallingInstance>(TCallingInstance instance,
            [CallerMemberName] string memberName = "")
        {
            _monitoring.ValidateCurrentCall(instance, memberName);
        }
    }
}

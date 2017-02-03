using System.Runtime.CompilerServices;
using LogManagement.Services;

namespace LogManagement
{
    public interface IActivityMonitoring
    {
        void ValidateCurrentCall<TCallingInstance>(TCallingInstance instance, [CallerMemberName] string memberName = "");
    }

    public class ActivityMonitoring : IActivityMonitoring
    {
        public void ValidateCurrentCall<TCallingInstance>(TCallingInstance instance, [CallerMemberName] string memberName = "")
        {
            string className = RegistrationService.GetInstance().GetClassName<TCallingInstance>();
        }
    }
}

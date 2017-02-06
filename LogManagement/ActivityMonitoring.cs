using LogManagement.Services;

namespace LogManagement
{
    public interface IActivityMonitoring
    {
        void ValidateCurrentCall<TCallingInstance>(TCallingInstance instance, string memberName);
    }

    public class ActivityMonitoring : IActivityMonitoring
    {
        public void ValidateCurrentCall<TCallingInstance>(TCallingInstance instance, string memberName)
        {
            string className = RegistrationService.GetInstance().GetClassName<TCallingInstance>();

        }
    }
}

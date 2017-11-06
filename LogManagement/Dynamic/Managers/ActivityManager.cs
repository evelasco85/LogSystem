using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LogManagement.Dynamic.Registration;

namespace LogManagement.Dynamic.Managers
{
    public interface IActivityManager
    {
        OnActivityEmitDelegate OnActivityEmit { get; set; }
        void RegisterSystem(ISystemRegistration systemRegistration);
        void EmitCurrentCallInfo<TCallingInstance>(TCallingInstance instance, [CallerMemberName] string memberName = "");
    }

    public delegate void OnActivityEmitDelegate(
        string systemName, string applicationName, string componentName, string eventName,
        IList<Tuple<string, object>> eventParameters);

    public class ActivityManager : IActivityManager
    {
        static IActivityManager s_instance = new ActivityManager();
        IActivityMonitoring _monitoring  = new ActivityMonitoring();
        private OnActivityEmitDelegate _onActivityEmit;

        private ActivityManager()
        {
        }

        public OnActivityEmitDelegate OnActivityEmit
        {
            get { return _onActivityEmit; }
            set { _onActivityEmit = value; }
        }

        public static IActivityManager GetInstance()
        {
            return s_instance;
        }

        public void RegisterSystem(ISystemRegistration systemRegistration)
        {
            _monitoring.AddSystem(systemRegistration);
        }

        public void EmitCurrentCallInfo<TCallingInstance>(TCallingInstance instance,
            [CallerMemberName] string memberName = "")
        {
            IList<IComponentRegistration> components = _monitoring.GetComponents(instance, memberName);

            if (_onActivityEmit == null)
                return;

            for (int index = 0; index < components.Count; index++)
            {
                IComponentRegistration component = components[index];
                IApplicationRegistration application = component.Application;
                ISystemRegistration system = application.SystemRegistration;
                string eventName = component.GetEventName(memberName);
                IList<Tuple<string, object>> parameters = component.GetObservedParameterValues(instance);

                _onActivityEmit(
                    system.Name,
                    application.Name,
                    component.Name,
                    (string.IsNullOrEmpty(eventName) ? memberName : eventName),
                    parameters);
            }
        }
    }
}

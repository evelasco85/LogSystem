using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using LogManagement.Registration;

namespace LogManagement.Managers
{
    public interface IActivityManager
    {
        ComponentInvocationDelegate ComponentInvocation { get; set; }
        void RegisterSystem(ISystemRegistration systemRegistration);
        void EmitCurrentCallInfo<TCallingInstance>(TCallingInstance instance, [CallerMemberName] string memberName = "");
    }

    public delegate void ComponentInvocationDelegate(
        string systemName, string applicationName, string componentName, string eventName,
        IList<Tuple<string, object>> eventParameters);

    public class ActivityManager : IActivityManager
    {
        static IActivityManager s_instance = new ActivityManager();
        IActivityMonitoring _monitoring  = new ActivityMonitoring();
        private ComponentInvocationDelegate _componentInvocation;

        private ActivityManager()
        {
        }

        public ComponentInvocationDelegate ComponentInvocation
        {
            get { return _componentInvocation; }
            set { _componentInvocation = value; }
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

            if (_componentInvocation == null)
                return;

            for (int index = 0; index < components.Count; index++)
            {
                IComponentRegistration component = components[index];
                IApplicationRegistration application = component.Application;
                ISystemRegistration system = application.SystemRegistration;
                string eventName = component.GetEventName(memberName);
                IList<Tuple<string, object>> parameters = component.GetObservedParameterValues(instance);

                _componentInvocation(
                    system.Name,
                    application.Name,
                    component.Name,
                    (string.IsNullOrEmpty(eventName) ? memberName : eventName),
                    parameters);
            }
        }
    }
}

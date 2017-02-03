using System;

namespace LogManagement
{
    public interface IEventRegistration<T>
    {
        IEventRegistration<T> RegisterEvent(string eventName, Func<T, Delegate> methodAssociation);
    }

    public interface IClassRegistration
    {
        IEventRegistration<T> RegisterClass<T>(string applicationName, string component);
    }

    public interface ISystemRegistration
    {
        IClassRegistration RegisterSystem(string systemName);
    }

    public interface IRegistration : ISystemRegistration
    {
    }
}

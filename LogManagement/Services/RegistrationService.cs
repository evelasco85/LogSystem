using System;

namespace LogManagement.Services
{
    public interface IRegistrationService
    {
        string GetClassName<T>();
        bool IsPrimitive<T>();
        bool IsPrimitive(Type propertyType);
    }

    public class RegistrationService : IRegistrationService
    {
        static IRegistrationService s_instance = new RegistrationService();

        private RegistrationService()
        {
        }

        public static IRegistrationService GetInstance()
        {
            return s_instance;
        }

        public string GetClassName<T>()
        {
            return typeof(T).FullName;
        }

        public bool IsPrimitive<T>()
        {
            return IsPrimitive(typeof(T));
        }

        public bool IsPrimitive(Type propertyType)
        {
            return (!propertyType.IsClass) || (propertyType == typeof(string));
        }
    }
}

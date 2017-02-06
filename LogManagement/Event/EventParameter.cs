using System.Reflection;

namespace LogManagement.Event
{
    public interface IEventParameter
    {
        string ParameterName { get; }
        void SetPropertyInfo(PropertyInfo propertyInfo);
        object GetValue(object instance);
    }

    public interface IEventParameter<T> : IEventParameter
    {
    }

    public class EventParameter : IEventParameter
    {
        private string _parameterName;
        private PropertyInfo _propertyInfo;

        public string ParameterName
        {
            get { return _parameterName; }
        }
        
        public EventParameter(string parameterName)
        {
            _parameterName = parameterName;

            //bool primitive = RegistrationService.GetInstance().IsPrimitive<T>();

            //if (!primitive)
            //    throw new ArgumentException("Only primitive types are required for event parameter");
        }

        public void SetPropertyInfo(PropertyInfo propertyInfo)
        {
            _propertyInfo = propertyInfo;
        }

        public object GetValue(object instance)
        {
            return _propertyInfo.GetValue(instance);;
        }
    }
}

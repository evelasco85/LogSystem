using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using LogManagement.Services;

namespace LogManagement.Registration
{
    public interface IComponentRegistration
    {
        string BusinessComponentName { get; }
        string ClassName { get; }
        string Identifier { get; }
    }

     public interface IComponentRegistration<T> : IComponentRegistration
     {
         IComponentRegistration<T> RegisterObservableEvent(string eventName, Expression<Func<T, Delegate>> methodExpression);
        IComponentRegistration<T> RegisterObservableParameter<TOut>(string parameterName, Expression<Func<T, TOut>> propertyExpression);
    }

    public class ComponentRegistration<T> : IComponentRegistration<T>
    {
        private string _businessComponentName = string.Empty;
        private string _className = string.Empty;

        public string BusinessComponentName { get { return _businessComponentName; } }
        public string ClassName { get { return _className; } }
        public string Identifier { get { return string.Format("{0}|{1}", _businessComponentName, _className); } }

        IDictionary<string, string> _eventDictionary = new Dictionary<string, string>();
        IDictionary<string, PropertyInfo> _parameterDictionary = new Dictionary<string, PropertyInfo>();
 
        public ComponentRegistration(string businessComponentName)
        {
            _businessComponentName = businessComponentName;
            _className = RegistrationService.GetInstance().GetClassName<T>();
        }

        public IComponentRegistration<T> RegisterObservableEvent(string eventName, Expression<Func<T, Delegate>> methodExpression)
        {
            if ((string.IsNullOrEmpty(eventName)) || (methodExpression == null))
                return this;

            if (_eventDictionary.ContainsKey(eventName))
                _eventDictionary.Remove(eventName);

            Delegate genericDelegate = methodExpression.Compile().Invoke(default(T));
            string methodName = genericDelegate.Method.Name;
            
            _eventDictionary.Add(eventName, methodName);

            return this;
        }

        public IComponentRegistration<T> RegisterObservableParameter<TOut>(string parameterName, Expression<Func<T, TOut>> propertyExpression)
        {
            if ((string.IsNullOrEmpty(parameterName)) || (propertyExpression == null))
                return this;

            PropertyInfo propertyInfo = ((MemberExpression)propertyExpression.Body).Member as PropertyInfo;

            if (propertyInfo == null)
            {
                throw new ArgumentException("'propertyExpression' parameter requires a valid property");
            }

            Type propertyType = typeof(TOut);
            bool primitive = (!propertyType.IsClass) || (propertyType == typeof(string));

            if(!primitive)
                throw new ArgumentException("Please register a primitive property");

            if (_parameterDictionary.ContainsKey(parameterName))
                _parameterDictionary.Remove(parameterName);

            _parameterDictionary.Add(parameterName, propertyInfo);

            return this;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using LogManagement.Dynamic.Services;

namespace LogManagement.Dynamic.Registration
{
    public interface IComponentRegistration : IRegistration
    {
        IApplicationRegistration Application { get; }
        string ClassName { get; }
        string Identifier { get; }

        string GetEventName(string member);
        bool MatchClass(string className);
        IList<Tuple<string, object>> GetObservedParameterValues<T>(T instance);
    }

    public interface IComponentRegistration<T> : IComponentRegistration
    {
        IComponentRegistration<T> RegisterObservableEvent(string eventName,
            Expression<Func<T, Delegate>> methodExpression);

        IComponentRegistration<T> RegisterObservableParameter<TOut>(string parameterName,
            Expression<Func<T, TOut>> propertyExpression);

        
    }

    public class ComponentRegistration<T> : Registration, IComponentRegistration<T>
    {
        private readonly string _className;
        private IApplicationRegistration _applicationRegistration;

        public string ClassName { get { return _className; } }
        public string Identifier { get { return string.Format("{0}|{1}", Name, _className); } }

        IDictionary<string, string> _eventDictionary = new Dictionary<string, string>();
        IDictionary<string, PropertyInfo> _parameterDictionary = new Dictionary<string, PropertyInfo>();

        public IApplicationRegistration Application
        {
            get { return _applicationRegistration; }
        }

        public ComponentRegistration(string businessComponentName, IApplicationRegistration application) : base(businessComponentName)
        {
            _className = RegistrationService.GetInstance().GetClassName<T>();
            _applicationRegistration = application;
        }

        public bool MatchClass(string className)
        {
            return className == _className;
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

            bool primitive = RegistrationService.GetInstance().IsPrimitive<TOut>();

            if(!primitive)
                throw new ArgumentException("Please register a primitive property");

            if (_parameterDictionary.ContainsKey(parameterName))
                _parameterDictionary.Remove(parameterName);

            _parameterDictionary.Add(parameterName, propertyInfo);

            return this;
        }

        public string GetEventName(string member)
        {
            if ((string.IsNullOrEmpty(member)) || (!_eventDictionary.Values.Contains(member)))
                return member;

            return _eventDictionary
                .Where(kvp => kvp.Value == member)
                .Select(kvp => kvp.Key)
                .FirstOrDefault();
        }

        public IList<Tuple<string, object>> GetObservedParameterValues<T2>(T2 instance)
        {
            IList<Tuple<string, object>> parameters = new List<Tuple<string, object>>();

            if (instance == null)
                return parameters;

            foreach (KeyValuePair<string, PropertyInfo> parameter in _parameterDictionary)
            {
                if(parameter.Value == null)
                    continue;

                Tuple<string, object> tValue = new Tuple<string, object>(
                    parameter.Key,
                    parameter.Value.GetValue(instance)
                    );

                parameters.Add(tValue);
            }

            return parameters;
        }
    }
}

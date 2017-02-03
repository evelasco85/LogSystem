using System;
using System.Linq.Expressions;

namespace LogManagement.Registration
{
    public interface IComponentRegistration
    {
    }

     public interface IComponentRegistration<T> : IComponentRegistration
    {
        IComponentRegistration<T> RegisterEvent(string eventName, Func<T, Delegate> methodAssociation);
        IComponentRegistration<T> RegisterObservableProperty<TOut>(string parameterName, Expression<Func<T, TOut>> property);
    }

    public class ComponentRegistration<T> : IComponentRegistration<T>
    {
        private string _businessComponentName = string.Empty;

        public ComponentRegistration(string businessComponentName)
        {
            _businessComponentName = businessComponentName;
        }

        public IComponentRegistration<T> RegisterEvent(string eventName, Func<T, Delegate> methodAssociation)
        {
            throw new NotImplementedException();

            return this;
        }

        public IComponentRegistration<T> RegisterObservableProperty<TOut>(string parameterName, Expression<Func<T, TOut>> property)
        {
            string propertyName = ((MemberExpression)property.Body).Member.Name;
            Type propertyType = ((MemberExpression) property.Body).Member.GetType();
            bool primitive = (!propertyType.IsClass) || (propertyType == typeof(string));

            if(!primitive)
                throw new ArgumentException("Please register a primitive property");

            throw new NotImplementedException();

            return this;
        }
    }
}

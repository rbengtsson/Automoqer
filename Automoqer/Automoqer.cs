using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;

namespace Automoqer
{
   public class AutoMoqer<T> : IDisposable
        where T : class
    {
        private readonly List<object> _parameters = new List<object>();
        private readonly List<object> _prameterMockInstances = new List<object>();
        private readonly Lazy<T> _serviceInstance;
        
        public AutoMoqer()
        {
            var constructors = typeof(T).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            var primaryConstructor = constructors.SingleOrDefault();

            if (primaryConstructor == null)
                throw new ArgumentException("Could not find a public constructor");

            var parameters = primaryConstructor.GetParameters();
            foreach (var parameter in parameters)
            {
                var genericType = typeof(Mock<>);
                var genericGenericType = genericType.MakeGenericType(parameter.ParameterType);
                var parameterInstance = Activator.CreateInstance(genericGenericType);

                _parameters.Add(parameterInstance);

                var parameterMockInstance = parameterInstance.GetType().GetProperty("Object", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).GetValue(parameterInstance, null);
                _prameterMockInstances.Add(parameterMockInstance);
            }

            _serviceInstance = new Lazy<T>(() => (T)Activator.CreateInstance(typeof(T), _prameterMockInstances.Cast<object>().ToArray()));
        }

        public Mock<TParam> Param<TParam>() where TParam : class
        {
            var genericType = typeof(Mock<>);
            var genericGenericType = genericType.MakeGenericType(typeof(TParam));

            var param = _parameters.SingleOrDefault(p => p.GetType() == genericGenericType);
            return (Mock<TParam>)Convert.ChangeType(param, typeof(Mock<TParam>));
        }

        public T Service => _serviceInstance.Value;

        public void Dispose()
        {
            foreach (var parameter in _parameters)
            {
                var method = parameter.GetType().GetMethod("VerifyAll");
                method.Invoke(parameter, null);
            }
        }
    }
}
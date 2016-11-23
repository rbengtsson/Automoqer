using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
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
            if(constructors.Length > 1)
                throw new ArgumentException("Multiple public constructors");

            var primaryConstructor = constructors.SingleOrDefault();
            if (primaryConstructor == null)
                throw new ArgumentException("Could not find a public constructor");

            var parameters = primaryConstructor.GetParameters();
            foreach (var parameter in parameters)
            {
                if(parameter.ParameterType.IsValueType)
                    throw new ArgumentException("Service constructor has an value type as parameter, not supported by Moq");

                var genericType = typeof(Mock<>);
                var genericGenericType = genericType.MakeGenericType(parameter.ParameterType);
                var parameterInstance = Activator.CreateInstance(genericGenericType);

                _parameters.Add(parameterInstance);

                var parameterMockInstance = parameterInstance.GetType().GetProperty("Object", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).GetValue(parameterInstance, null);
                _prameterMockInstances.Add(parameterMockInstance);
            }

            _serviceInstance = new Lazy<T>(() => (T)Activator.CreateInstance(typeof(T), _prameterMockInstances.Cast<object>().ToArray()));
        }

        /// <summary>
        /// Get constructor parameter Mock instance
        /// </summary>
        /// <typeparam name="TParam">Type of constructor parameter</typeparam>
        /// <returns>A mock instance of type TParam, or null of no such parameter could be resolved</returns>
        public Mock<TParam> Param<TParam>() where TParam : class
        {
            var genericType = typeof(Mock<>);
            var genericGenericType = genericType.MakeGenericType(typeof(TParam));

            var param = _parameters.SingleOrDefault(p => p.GetType() == genericGenericType);
            if (param == null)
                return null;

            return (Mock<TParam>)Convert.ChangeType(param, typeof(Mock<TParam>));
        }

        public T Service => _serviceInstance.Value;

        public void Dispose()
        {
			var exceptionOccurred = Marshal.GetExceptionPointers() != IntPtr.Zero
						|| Marshal.GetExceptionCode() != 0;

			if(exceptionOccurred)
			{
				return;
			}

			foreach (var parameter in _parameters)
            {
                var method = parameter.GetType().GetMethod("VerifyAll");
                method.Invoke(parameter, null);
            }
        }
    }
}
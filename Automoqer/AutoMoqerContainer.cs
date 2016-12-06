using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using Moq;

namespace Automoqer
{
    public class AutoMoqerContainer<TService> : IDisposable
    {
        private readonly List<object> _moqInstancesParameters = new List<object>();        
        private readonly Lazy<TService> _serviceInstance;

        internal AutoMoqerContainer(
            ConstructorInfo primaryConstructor,
            IReadOnlyDictionary<Type, object> exceptionParametersByType,
            IReadOnlyDictionary<string, object> exceptionParametersByName)
        {
            var serviceConstructionParameters = new List<object>();

            var parameters = primaryConstructor.GetParameters();
            foreach (var parameter in parameters)
            {
                var exceptionByType = exceptionParametersByType.ContainsKey(parameter.ParameterType);
                var exceptionByName = exceptionParametersByName.ContainsKey(parameter.Name.ToLower());
                if(exceptionByType && exceptionByName)
                    throw new ArgumentException($"Parameter named {parameter.Name} has multiple registered exceptions (both by type and by name)");

                if (exceptionByName)
                {
                    serviceConstructionParameters.Add(exceptionParametersByName.First(p => p.Key == parameter.Name.ToLower()).Value);
                }
                else if(exceptionByType)
                {
                    serviceConstructionParameters.Add(exceptionParametersByType.First(p => p.Key == parameter.ParameterType).Value);
                }
                else
                {                    
                    if (parameter.ParameterType.IsValueType)
                        throw new ArgumentException($"Unable to create Moq-object for parameter named {parameter.Name} as Moq doesn't support value-types");

                    //Create and add Moq-instance for parameter
                    var genericType = typeof(Mock<>);
                    var genericGenericType = genericType.MakeGenericType(parameter.ParameterType);
                    var parameterInstance = Activator.CreateInstance(genericGenericType);
                    _moqInstancesParameters.Add(parameterInstance);

                    var parameterMockInstance = parameterInstance.GetType().GetProperty("Object", BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance).GetValue(parameterInstance, null);
                    serviceConstructionParameters.Add(parameterMockInstance);
                }
            }

            _serviceInstance = new Lazy<TService>(() => (TService)Activator.CreateInstance(typeof(TService), serviceConstructionParameters.Cast<object>().ToArray()));
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

            var param = _moqInstancesParameters.SingleOrDefault(p => p.GetType() == genericGenericType);
            if (param == null)
                return null;

            return (Mock<TParam>)Convert.ChangeType(param, typeof(Mock<TParam>));
        }

        /// <summary>
        /// Instance to the service instance
        /// </summary>
        public TService Service => _serviceInstance.Value;

        /// <summary>
        /// Will run VerifyAll on all Moq-parameters
        /// </summary>
        public void Dispose()
        {
            var exceptionOccurred = Marshal.GetExceptionPointers() != IntPtr.Zero || Marshal.GetExceptionCode() != 0;

            if (exceptionOccurred)
            {
                return;
            }

            foreach (var parameter in _moqInstancesParameters)
            {
                var method = parameter.GetType().GetMethod("VerifyAll");
                method.Invoke(parameter, null);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Moq;

namespace Automoqer
{
    /// <summary>
    /// Represents the base class of the container for AutoMoqer that holds all constructor dependencies for a service and allows verifying all expectations.
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqerContainerBase<TService>
    {
        private readonly List<object> _moqInstancesParameters = new List<object>();
        private readonly Lazy<TService> _serviceInstance;

        internal AutoMoqerContainerBase(
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
                if (exceptionByType && exceptionByName)
                    throw new ArgumentException($"Parameter named {parameter.Name} has multiple registered exceptions (by type and/or by name)");

                if (exceptionByName)
                {
                    serviceConstructionParameters.Add(exceptionParametersByName.First(p => p.Key == parameter.Name.ToLower()).Value);
                }
                else if (exceptionByType)
                {
                    serviceConstructionParameters.Add(exceptionParametersByType.First(p => p.Key == parameter.ParameterType).Value);
                }
                else
                {
                    if (parameter.GetType().GetTypeInfo().IsValueType)
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

            //Create new service with mock parameter and provided parameters
            _serviceInstance = new Lazy<TService>(() => (TService)Activator.CreateInstance(typeof(TService), serviceConstructionParameters.ToArray()));
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
                throw new ArgumentException($"Parameter with type {typeof(TParam).Name} not found in constructor parameter mock list");

            return (Mock<TParam>)Convert.ChangeType(param, typeof(Mock<TParam>));
        }

        /// <summary>
        /// Instance to the service instance
        /// </summary>
        public TService Service => _serviceInstance.Value;

        /// <summary>
        /// Creates the service.
        /// </summary>
        public void CreateService()
        {
            var unused = _serviceInstance.Value;
        }

        /// <summary>
        /// Will run VerifyAll on all Moq-parameters
        /// </summary>
        protected void VerifyAllInstances()
        {
            foreach (var parameter in _moqInstancesParameters)
            {
                var method = parameter.GetType().GetMethod("VerifyAll");
                method.Invoke(parameter, null);
            }
        }
    }
}

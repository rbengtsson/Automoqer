using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Automoqer
{
    /// <summary>
    /// AutoMoqer mocks all constructor dependencies for a service
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqer<TService> where TService : class
    {
        private readonly ConstructorInfo _primaryConstructor;
        private readonly Dictionary<Type, object> _exceptionParametersByType = new Dictionary<Type, object>();
        private readonly Dictionary<string, object> _exceptionParametersByName = new Dictionary<string, object>();

        /// <summary>
        /// Create a new AutoMoqer instance for a service with type TService
        /// </summary>
        public AutoMoqer()
        {
            var constructors = typeof(TService).GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            if(constructors.Length > 1)
                throw new ArgumentException("Multiple public constructors found");

            var primaryConstructor = constructors.FirstOrDefault();
            if (primaryConstructor == null)
                throw new ArgumentException("Could not find a public constructor");

            _primaryConstructor = primaryConstructor;
        }

        /// <summary>
        /// Use instance for type TParam instead of creating a Mock-object for that paramter
        /// </summary>
        /// <typeparam name="TParam">The type of parameter to replace</typeparam>
        /// <param name="instance">The value to use instead of a Mock-object</param>
        /// <returns>An instance to the current AutoMoqer object</returns>
        public AutoMoqer<TService> With<TParam>(object instance)
        {
            if(_exceptionParametersByType.ContainsKey(typeof(TParam)))
                throw new ArgumentException($"An instance for the parameter with type {typeof(TParam).Name} has already been registered");

            _exceptionParametersByType.Add(typeof(TParam), instance);
            return this;
        }

        /// <summary>
        /// Use instance for parameter named name instead of creating a Mock-object
        /// </summary>
        /// <param name="name">The type of parameter to replace</param>
        /// <param name="instance">The value to use instead of a Mock-object</param>
        /// <returns>An instance to the current AutoMoqer object</returns>
        public AutoMoqer<TService> With(string name, object instance)
        {
            var nameLowerCase = name.ToLower();

            if (_exceptionParametersByName.ContainsKey(nameLowerCase))
                throw new ArgumentException($"An instance for the parameter named {name} has already been registered");

            _exceptionParametersByName.Add(nameLowerCase, instance);
            return this;
        }

#if NET46
        /// <summary>
        /// Create a new <see cref="AutoMoqer{TService}"/> container with the current configuration.
        /// </summary>
        /// <returns>A new <see cref="AutoMoqer{TService}"/> container</returns>
        public AutoMoqerContainer<TService> Build()
        {
            //Clone the lists to support the creation of multiple independent containers from the same AutoMoqer-object
            var exceptionParametersByTypeCopy = new Dictionary<Type, object>(_exceptionParametersByType);
            var exceptionParametersByNameCopy = new Dictionary<string, object>(_exceptionParametersByName);

            return new AutoMoqerContainer<TService>(
                _primaryConstructor,
                exceptionParametersByTypeCopy,
                exceptionParametersByNameCopy);
        }
#endif

        /// <summary>
        /// Create a new <see cref="AutoMoqerContainerWithExplicitVerification{TService}"/> container with the current configuration.
        /// </summary>
        /// <returns>A new <see cref="AutoMoqerContainerWithExplicitVerification{TService}"/> container</returns>
        public AutoMoqerContainerWithExplicitVerification<TService> BuildWithExplicitVerification()
        {
            //Clone the lists to support the creation of multiple independent containers from the same AutoMoqer-object
            var exceptionParametersByTypeCopy = new Dictionary<Type, object>(_exceptionParametersByType);
            var exceptionParametersByNameCopy = new Dictionary<string, object>(_exceptionParametersByName);

            return new AutoMoqerContainerWithExplicitVerification<TService>(
                _primaryConstructor,
                exceptionParametersByTypeCopy,
                exceptionParametersByNameCopy);
        }
    }
}
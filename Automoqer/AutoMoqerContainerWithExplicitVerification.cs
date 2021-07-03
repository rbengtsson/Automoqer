using System;
using System.Collections.Generic;
using System.Reflection;

namespace Automoqer
{
    /// <summary>
    /// A container for AutoMoqer that holds all constructor dependencies for a service and allows verifying all expectations.
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqerContainerWithExplicitVerification<TService> : AutoMoqerContainerBase<TService>
    {
        protected internal AutoMoqerContainerWithExplicitVerification(
            ConstructorInfo primaryConstructor,
            IReadOnlyDictionary<Type, object> exceptionParametersByType,
            IReadOnlyDictionary<string, object> exceptionParametersByName)
            : base(primaryConstructor, exceptionParametersByType, exceptionParametersByName)
        {
            // nothing to do here
        }

        /// <summary>
        /// Will run <see cref="Moq.Mock.VerifyAll"/> on all Moq-parameters.
        /// </summary>
        public void VerifyAll()
        {
            VerifyAllInstances();
        }
    }
}
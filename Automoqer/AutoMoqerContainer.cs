#if NET46

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Automoqer
{
    /// <summary>
    /// A container for AutoMoqer that holds all constructor dependencies for a service
    /// </summary>
    /// <typeparam name="TService">Type of the service to mock dependencies for</typeparam>
    public class AutoMoqerContainer<TService> : AutoMoqerContainerBase<TService>, IDisposable
    {
        internal AutoMoqerContainer(
            ConstructorInfo primaryConstructor,
            IReadOnlyDictionary<Type, object> exceptionParametersByType,
            IReadOnlyDictionary<string, object> exceptionParametersByName)
            : base(primaryConstructor, exceptionParametersByType, exceptionParametersByName)
        {
            // nothing to do here
        }

        /// <summary>
        /// Will run VerifyAll on all Moq-parameters
        /// </summary>
        public void Dispose()
        {
            var exceptionOccurred = Marshal.GetExceptionPointers() != IntPtr.Zero || Marshal.GetExceptionCode() != 0;
            if (exceptionOccurred)
                return;

            VerifyAllInstances();
        }
    }
}

#endif

using Automoqer.Test.Model.Interfaces;

namespace Automoqer.Test.Model
{
    public class ServiceWithReferenceTypeParameter
    {
        public IReferenceTypeParameter ReferenceTypeParameter;

        public ServiceWithReferenceTypeParameter(IReferenceTypeParameter referenceTypeParameter)
        {
            ReferenceTypeParameter = referenceTypeParameter;
        }
    }
}

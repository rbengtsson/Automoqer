using Automoqer.Test.Model.Interfaces;

namespace Automoqer.Test.Model
{
    public class ServiceWithMultipleConstructors
    {
        public ServiceWithMultipleConstructors()
        {
            
        }

        private readonly ISimpleService _simpleService;
        public ServiceWithMultipleConstructors(ISimpleService simpleService)
            : this()
        {
            _simpleService = simpleService;
        }
    }
}
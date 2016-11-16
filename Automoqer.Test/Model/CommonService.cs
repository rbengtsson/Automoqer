using Automoqer.Test.Model.Interfaces;

namespace Automoqer.Test.Model
{
    public class CommonService
    {
        private readonly ISimpleService _simpleService;
        public CommonService(ISimpleService simpleService)
        {
            _simpleService = simpleService;
        }

        bool TestMethod(string input)
        {
            _simpleService.SetString(input);
            return _simpleService.GetBool();
        }
    }
}
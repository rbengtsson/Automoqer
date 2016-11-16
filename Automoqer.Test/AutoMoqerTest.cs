using Automoqer.Test.Model;
using Automoqer.Test.Model.Interfaces;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Automoqer.Test
{
    [TestClass]
    public class AutoMoqerTest
    {
        [TestMethod]        
        public void AutoMoqerCanBeCreatedWithNormalConstructor()
        {
            var automoqer = new AutoMoqer<CommonService>();

            //Assert result
            Assert.IsNotNull(automoqer);

            //Assert parameter
            var parameter = automoqer.Param<ISimpleService>();
            Assert.IsNotNull(parameter);
            Assert.IsInstanceOfType(parameter, typeof(Mock<ISimpleService>));

            //Assert service
            Assert.IsNotNull(automoqer.Service);
            Assert.IsInstanceOfType(automoqer.Service, typeof(CommonService));
        }
    }
}

using System;
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
        public void AutoMoqerCanBeCreatedWithNormalConstructorSuccessfullt()
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

        [TestMethod]
        public void AutoMoqerCanBeCreatedWithEmptyConstructor()
        {
            var automoqer = new AutoMoqer<ServiceWithNoConstructorParameters>();

            //Assert result
            Assert.IsNotNull(automoqer);

            //Assert parameter
            var parameter = automoqer.Param<ISimpleService>();
            Assert.IsNull(parameter);

            //Assert service
            Assert.IsNotNull(automoqer.Service);
            Assert.IsInstanceOfType(automoqer.Service, typeof(ServiceWithNoConstructorParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerCanNotBeCreatedWithMultiplePublicConstructorsThrowsException()
        {
            var automoqer = new AutoMoqer<ServiceWithMultipleConstructors>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerCanNotBeCreatedWithNoPublicConstructorThrowsException()
        {
            var automoqer = new AutoMoqer<ServiceWithNoPublicConstructor>();
        }
    }
}

using System;
using Automoqer.Test.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Automoqer.Test
{
    [TestClass]
    public class AutoMoqerParameterExceptionTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerCanNotBeCreatedWithValueTypeInConstructorWithNoExceptionsThrowsException()
        {
            var automoqer = new AutoMoqer<ServiceWithValueTypeInConstructor>().Build();
        }

        [TestMethod]
        public void AutoMoqerCanConstructServiceWithValueTypeHasExceptionByName()
        {
            using (var automoqer = new AutoMoqer<ServiceWithValueTypeInConstructor>()
                .With("intVal", 7)
                .Build())
            {
                var val = automoqer.Service.GetVal();
                Assert.AreEqual(7, val);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerThrowsExceptionWhenRegisteredMultipleIdenticalParametersByName()
        {
            using (var automoqer = new AutoMoqer<ServiceWithValueTypeInConstructor>()
                .With("intVal", 7)
                .With("intVal", 6)
                .Build())
            {
                automoqer.Service.GetVal();
            }
        }

        [TestMethod]
        public void AutoMoqerCanConstructServiceWithValueTypeHasExceptionByType()
        {
            using (var automoqer = new AutoMoqer<ServiceWithValueTypeInConstructor>()
                .With<int>(6)
                .Build())
            {
                var val = automoqer.Service.GetVal();
                Assert.AreEqual(6, val);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerThrowsExceptionWhenRegisteredMultipleIdenticalParametersByType()
        {
            using (var automoqer = new AutoMoqer<ServiceWithValueTypeInConstructor>()
                .With<int>(7)
                .With<int>(6)
                .Build())
            {
                automoqer.Service.GetVal();
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerThrowsExceptionWhenRegisteredMultipleIdenticalParametersByTypeAndName()
        {
            using (var automoqer = new AutoMoqer<ServiceWithValueTypeInConstructor>()
                .With("intVal", 6)
                .With<int>(6)
                .Build())
            {
                automoqer.Service.GetVal();
            }
        }
    }
}
﻿#if NET46

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
            var automoqer = new AutoMoqer<CommonService>().Build();

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
            var automoqer = new AutoMoqer<ServiceWithNoConstructorParameters>().Build();

            //Assert result
            Assert.IsNotNull(automoqer);

            //Assert parameter
            try
            {
                automoqer.Param<ISimpleService>();
                Assert.Fail("Should have thrown exception");
            }
            catch (Exception ex)
            {
                Assert.IsInstanceOfType(ex, typeof(ArgumentException));
            }

            //Assert service
            Assert.IsNotNull(automoqer.Service);
            Assert.IsInstanceOfType(automoqer.Service, typeof(ServiceWithNoConstructorParameters));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerCanNotBeCreatedWithMultiplePublicConstructorsThrowsException()
        {
            var automoqer = new AutoMoqer<ServiceWithMultipleConstructors>().Build();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerCanNotBeCreatedWithNoPublicConstructorThrowsException()
        {
            var automoqer = new AutoMoqer<ServiceWithNoPublicConstructor>().Build();
        }

        [TestMethod]
        public void AutoMoqerThrowsExceptionOnDisposeForNonCalledMethods()
        {
            try
            {
                using(var automoqer = new AutoMoqer<CommonService>().Build())
                {
                    automoqer
                        .Param<ISimpleService>()
                        .Setup(f => f.GetBool())
                        .Returns(true);
                }

                // Should not happen.
                Assert.IsFalse(true);
            }
            catch(System.Reflection.TargetInvocationException exc)
            {
                // Moq.MockVerificationException is internal.
                Assert.AreEqual("Moq.MockVerificationException", exc.InnerException.GetType().FullName);
            }
        }

        [TestMethod]
        public void AutoMoqerWithExplicitVerificationCallThrowsExceptionForNonCalledMethods()
        {
            try
            {
                var automoqer = new AutoMoqer<CommonService>().BuildWithExplicitVerification();
                automoqer
                    .Param<ISimpleService>()
                    .Setup(f => f.GetBool())
                    .Returns(true);
                automoqer.VerifyAll();
            }
            catch (System.Reflection.TargetInvocationException exc)
            {
                // Moq.MockVerificationException is internal.
                Assert.AreEqual("Moq.MockVerificationException", exc.InnerException.GetType().FullName);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AutoMoqerShouldThrowCorrectExceptionWithUsingStatement()
        {
            using(var automoqer = new AutoMoqer<CommonService>().Build())
            {
                automoqer
                    .Param<ISimpleService>()
                    .Setup(f => f.SetString(It.IsAny<string>()));

                throw new ArgumentException();
            }
        }
    }
}

#endif

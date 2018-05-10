﻿#if NET46

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Automoqer.Test
{
    [TestClass]
    public class AutoMoqerContainerTests
    {
        [TestMethod]
        public void CreateServiceShouldCreateTheInstance()
        {
            // Arrange
            using (var autoMoqer = new AutoMoqer<Foo>()
                .Build())
            {
                // Act
                autoMoqer.CreateService();

                // Assert
                Assert.IsNotNull(autoMoqer.Service);
            }
        }

        private class Foo
        {
            // nothing to do here
        }
    }
}

#endif

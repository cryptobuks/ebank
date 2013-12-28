﻿using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Presentation.Controllers;

namespace Presentation.Tests.Controllers
{
    [TestClass]
    public class StatisticControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            var controller = new StatisticController();

            // Act
            var result = controller.LoanApplication() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}

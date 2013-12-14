using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Presentation.Controllers;

namespace Presentation.Tests.Controllers
{
    [TestClass]
    public class CommitteeControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Arrange
            var controller = new CommitteeController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}

using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Presentation.Controllers;

namespace Presentation.Tests.Controllers
{
    [TestClass]
    public class TariffsControllerTest
    {
        [TestMethod]
        public void Index()
        {
            // Mock
            var mock = new Mock<ControllerContext>();
            mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("AnonymousUser");
            mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
            mock.Setup(p => p.HttpContext.User.IsInRole("Department head")).Returns(false);

            // Arrange
            var controller = new TariffsController { ControllerContext = mock.Object };

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}

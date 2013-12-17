using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Presentation.Controllers;

namespace Presentation.Tests.Controllers
{
    [TestClass]
    public class LoanApplicationControllerTest
    {
        private static readonly string[] RoleNames = {"Customer", "Consultant", "Operator", "Security", "Credit committee", "Department head"};
        private static readonly string[] EmployeeRoles = RoleNames.Skip(1).ToArray();

        [TestMethod]
        public void Index()
        {
            foreach (var roleName in EmployeeRoles)
            {
                var role = roleName;

                // Mock
                var mock = new Mock<ControllerContext>();
                mock.SetupGet(p => p.HttpContext.User.Identity.Name).Returns("User");
                mock.SetupGet(p => p.HttpContext.Request.IsAuthenticated).Returns(true);
                mock.Setup(p => p.HttpContext.User.IsInRole(role)).Returns(true);

                // Arrange
                var controller = new LoanApplicationController { ControllerContext = mock.Object };

                // Act
                var result = controller.Index();

                // Assert
                Assert.IsNotNull(result);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void AnonymousIndex()
        {
            // Arrange
            var controller = new LoanApplicationController();

            // Act
            var result = controller.Index() as ViewResult;

            // Assert
            Assert.IsNotNull(result);
        }
    }
}

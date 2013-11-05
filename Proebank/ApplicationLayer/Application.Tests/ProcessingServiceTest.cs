using System;
using Application.AccountProcessing;
using Application.LoanProcessing;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.Tests
{
    [TestClass]
    public class ProcessingServiceTest
    {
        private static UnityContainer _container;
        private static ProcessingService _service;

        [ClassInitialize]
        public static void  InitService(TestContext context)
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
            _service = _container.Resolve<ProcessingService>();
        }

        [TestMethod]
        public void ProcessEndOfMonth()
        {
            // TODO: add data!
            _service.ProcessEndOfMonth(DateTime.UtcNow);
        }

        //[TestMethod]
        //public void CreateLoanContract()
        //{
        //    // TODO: add data!
        //    var loan = _service.CreateLoanContract(new LoanApplication
        //    {
        //        CellPhone = String.Empty,
        //        Documents = null,
        //        LoanAmount = 1000000,
        //    });
        //    Assert.IsNotNull(loan);
        //    // TODO: add check of accounts and so on
        //}
    }
}

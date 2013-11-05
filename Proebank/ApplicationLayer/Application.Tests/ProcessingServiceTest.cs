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
        private static LoanService _loanService;
        private static AccountService _accountService;

        [ClassInitialize]
        public static void  InitService(TestContext context)
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
            _accountService = _container.Resolve<AccountService>();
            _loanService = _container.Resolve<LoanService>();
        }

        [TestMethod]
        public void ProcessEndOfMonth()
        {
            // TODO: add data!
            ProcessingService.ProcessEndOfMonth(DateTime.UtcNow, _accountService, _loanService);
        }
    }
}

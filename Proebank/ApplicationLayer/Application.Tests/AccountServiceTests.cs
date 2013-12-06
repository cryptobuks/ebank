using Domain.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        private static ProcessingService _service;

        [ClassInitialize]
        public static void InitService(TestContext context)
        {
            _service = new ProcessingService();
        }

        [TestMethod]
        public void CreateAccount()
        {
            var account = _service.CreateAccount(Currency.BYR, AccountType.ContractService);
            Assert.IsNotNull(account);
        }

        //[TestMethod]
        //public void AddEntry()
        //{
            
        //}

        //[TestMethod]
        //public void CloseAccount()
        //{
            
        //}
    }
}

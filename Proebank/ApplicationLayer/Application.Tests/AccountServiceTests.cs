using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Repositories;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        private static AccountRepository _service;
        private static IUnityContainer _container;

        [ClassInitialize]
        public static void InitService(TestContext context)
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
            _service = _container.Resolve<AccountRepository>();
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

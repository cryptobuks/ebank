using Application.AccountProcessing;
using Domain.Enums;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RepositoriesContracts;

namespace Application.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        private static AccountService _service;
        private static IUnityContainer _container;

        [ClassInitialize]
        public static void InitService(TestContext context)
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
            var _repository = _container.Resolve<IAccountRepository>();
            _service = new AccountService(_repository);
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

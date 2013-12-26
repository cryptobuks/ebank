using Domain.Enums;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.Tests
{
    [TestClass]
    public class AccountServiceTests
    {
        [Dependency]
        protected ProcessingService Service { get; set; }

        //[ClassInitialize]
        //public static void InitService(TestContext context)
        //{
        //    Service = new ProcessingService();
        //}

        [TestMethod]
        public void CreateAccount()
        {
            var account = Service.CreateAccount(Currency.BYR, AccountType.ContractService);
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

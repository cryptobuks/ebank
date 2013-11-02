using ApplicationLayer.AccountProcessing;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            //var section = (UnityConfigurationSection)ConfigurationManager.GetSection("unity");
            //section.Configure(_container);
            _container.LoadConfiguration();
            _service = _container.Resolve<AccountService>();
        }

        [TestMethod]
        public void CreateAccount()
        {
            var account = _service.CreateAccount();
            Assert.IsNotNull(account);
        }
    }
}

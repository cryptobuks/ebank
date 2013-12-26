using System;
using System.Collections.ObjectModel;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.Tests
{
    /// <summary>
    /// Summary description for LoanServiceTest
    /// </summary>
    [TestClass]
    public class LoanServiceTest
    {
        [Dependency]
        protected ProcessingService Service { get; set; }
        private static Customer _customer;
        private static PersonalData _passport;
        private static Tariff _tariff;
        private static LoanApplication _validLoanApp;
        private static LoanApplication _invalidLoanApp;

        //[ClassInitialize]
        //public static void InitService(TestContext context)
        //{
        //    var bankAccountByr = _service.CreateAccount(Currency.BYR, AccountType.BankBalance);
        //    _service.AddEntry(bankAccountByr, new Entry()
        //    {
        //        Amount = 1E14M,
        //        Date = DateTime.UtcNow,
        //        Currency = Currency.BYR,
        //        Type = EntryType.Capital,
        //        SubType = EntrySubType.CharterCapital
        //    });
        //    _customer = new Customer
        //    {
        //        UserName = "test_customer",
        //        Email = null,
        //        Phone = "+375111111111",
        //    };
        //    _passport = new PersonalData
        //    {
        //        Customer = _customer,
        //        Address = "Minsk",
        //        LastName = "Mitchell",
        //        FirstName = "Stanley",
        //        MiddleName = "Matthew",
        //        DateOfBirth = new DateTime(1972, 10, 17),
        //        Identification = "317041972A0PB1",
        //        Passport = "MP2345678"
        //    };
        //    _customer.PersonalData = _passport;
        //    _tariff = new Tariff
        //    {
        //        CreationDate = new DateTime(2013, 07, 01),
        //        Currency = Currency.BYR,
        //        IsActive = true,
        //        InterestRate = 0.75M,
        //        IsGuarantorNeeded = false,
        //        LoanPurpose = LoanPurpose.Common,
        //        MaxAmount = 1.0E8M,
        //        MinAge = 18,
        //        MaxTerm = 36,
        //        MinTerm = 3,
        //        MinAmount = 1.0E6M,
        //        Name = "NeverSeeMeAgain"
        //    };
        //    _service.UpsertTariff(_tariff);
        //    _validLoanApp = new LoanApplication
        //    {
        //        CellPhone = "+375291000000",
        //        Currency = Currency.BYR,
        //        LoanAmount = 5.5E7M,
        //        LoanPurpose = LoanPurpose.Common,
        //        PersonalData = _passport,
        //        Tariff = _tariff,
        //        TariffId = _tariff.Id,
        //        Term = 3,
        //        TimeCreated = DateTime.Now
        //    };
        //    _invalidLoanApp = new LoanApplication
        //    {
        //        CellPhone = "+375291000000",
        //        Currency = Currency.BYR,
        //        LoanAmount = 5.5E11M,
        //        LoanPurpose = LoanPurpose.Common,
        //        PersonalData = _passport,
        //        Tariff = _tariff,
        //        TariffId = _tariff.Id,
        //        Term = 120,
        //        TimeCreated = DateTime.Now
        //    };
        //}

        [TestMethod]
        public void CreateValidLoanApplication()
        {
            Service.CreateLoanApplication(_validLoanApp);
            // if something fails here, there will be an exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Exception about invalid loan application should have been thrown")]
        public void CreateInvalidLoanApplication()
        {
            Service.CreateLoanApplication(_invalidLoanApp);
        }

        [TestMethod]
        public void ApproveLoanApplication()
        {
            Service.ApproveLoanAppication(_validLoanApp);
            Assert.AreEqual(LoanApplicationStatus.Approved, _validLoanApp.Status);
        }

        [TestMethod]
        public void RejectLoanApplication()
        {
            Service.RejectLoanApplication(_validLoanApp);
            Assert.AreEqual(LoanApplicationStatus.Rejected, _validLoanApp.Status);
        }
    }
}

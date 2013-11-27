using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using Application.LoanProcessing;
using Domain;
using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Repositories;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Application.Tests
{
    /// <summary>
    /// Summary description for LoanServiceTest
    /// </summary>
    [TestClass]
    public class LoanServiceTest
    {
        private static LoanRepository _service;
        private static IUnityContainer _container;
        private static Customer _customer;
        private static Document _passport;
        private static Tariff _tariff;
        private static LoanApplication _validLoanApp;
        private static LoanApplication _invalidLoanApp;

        [ClassInitialize]
        public static void InitService(TestContext context)
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
            _service = _container.Resolve<LoanRepository>();
            _customer = new Customer
            {
                UserName = "test_customer",
                LastName = "Mitchell",
                FirstName = "Stanley",
                MiddleName = "Matthew",
                Address = "Minsk",
                BirthDate = new DateTime(1972, 10, 17),
                Email = null,
                IdentificationNumber = "317041972A0PB1",
                Phone = "+375111111111",
            };
            _passport = new Document
            {
                CustomerId = _customer.Id,
                Customer = _customer,
                DocType = DocType.Passport,
                TariffDocType = TariffDocType.DebtorPrimary,
                Number = "MP2345678"
            };
            _tariff = new Tariff
            {
                CreationDate = new DateTime(2013, 07, 01),
                EndDate = null,
                InitialFee = 0,
                InterestRate = 0.75M,
                IsGuarantorNeeded = false,
                LoanPurpose = LoanPurpose.Common,
                MaxAmount = 1.0E8M,
                MinAge = 18,
                MaxTerm = 36,
                MinTerm = 3,
                MinAmount = 1.0E6M,
                Name = "NeverSeeMeAgain"
            };
            _service.UpsertTariff(_tariff);
            _validLoanApp = new LoanApplication
            {
                CellPhone = "+375291000000",
                Documents = new Collection<Document> { _passport },
                LoanAmount = 5.5E7M,
                LoanPurpose = LoanPurpose.Common,
                Tariff = _tariff,
                Term = 3,
                TimeCreated = DateTime.Now
            };
            _invalidLoanApp = new LoanApplication
            {
                CellPhone = "+375291000000",
                Documents = new Collection<Document> { _passport },
                LoanAmount = 5.5E11M,
                LoanPurpose = LoanPurpose.Common,
                Tariff = _tariff,
                Term = 120,
                TimeCreated = DateTime.Now
            };
        }

        [TestMethod]
        public void CreateValidLoanApplication()
        {
            _service.CreateLoanApplication(_validLoanApp);
            // if something fails here, there will be an exception
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException), "Exception about invalid loan application should have been thrown")]
        public void CreateInvalidLoanApplication()
        {
            _service.CreateLoanApplication(_invalidLoanApp);
        }

        [TestMethod]
        public void ConsiderLoanApplication()
        {
            _service.ConsiderLoanApplication(_validLoanApp, true);
            Assert.AreEqual(LoanApplicationStatus.Approved, _validLoanApp.Status);
        }
    }
}

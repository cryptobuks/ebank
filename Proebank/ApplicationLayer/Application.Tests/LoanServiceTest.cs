using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using Application.LoanProcessing;
using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Loans;
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
        private static LoanService _service;
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
            var loanRepository = _container.Resolve<ILoanRepository>();
            var loanApplicationRepository = _container.Resolve<ILoanApplicationRepository>();
            var tariffRepository = _container.Resolve<ITariffRepository>();
            _service = new LoanService(loanRepository, loanApplicationRepository, tariffRepository);
            _customer = new Customer
            {
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
                IsSecondaryDocumentNeeded = false,
                LoanPurpose = LoanPurpose.Common,
                MaxAmount = 1.0E8M,
                MinAge = 18,
                MaxTerm = 36,
                MinTerm = 3,
                MinAmount = 1.0E6M,
                Name = "NeverSeeMeAgain"
            };
            tariffRepository.SaveOrUpdate(_tariff);
            _validLoanApp = new LoanApplication
            {
                CellPhone = "+375291000000",
                Documents = new Collection<Document> { _passport },
                LoanAmount = 5.5E7M,
                LoanPurpose = LoanPurpose.Common,
                TariffId = _tariff.Id,
                Term = 3,
                TimeCreated = DateTime.Now
            };
            _invalidLoanApp = new LoanApplication
            {
                CellPhone = "+375291000000",
                Documents = new Collection<Document> { _passport },
                LoanAmount = 5.5E11M,
                LoanPurpose = LoanPurpose.Common,
                TariffId = _tariff.Id,
                Term = 120,
                TimeCreated = DateTime.Now
            };
        }

        [TestMethod]
        public void CreateValidLoanApplication()
        {
            Assert.IsTrue(_service.CreateLoanApplication(_validLoanApp));
        }

        [TestMethod]
        public void CreateInvalidLoanApplication()
        {
            Assert.IsFalse(_service.CreateLoanApplication(_invalidLoanApp));
        }
    }
}

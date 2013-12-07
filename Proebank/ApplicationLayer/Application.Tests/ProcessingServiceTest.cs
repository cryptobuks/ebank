using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Application.Tests
{
    [TestClass]
    public class ProcessingServiceTest
    {
        private ProcessingService _service;
        private Loan _loan;
        private LoanApplication _validLoanApp;
        private Tariff _tariff;
        private Document _passport;
        private Customer _customer;

        [TestInitialize]
        public void InitService()
        {
            _service = new ProcessingService();
            _service.SetCurrentDate(new DateTime(2013, 12, 07));
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
            //_service.SaveOrUpdateTariff(tariff);
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
            // TODO: check 2 cases: existing user and new
            _loan = _service.CreateLoanContract(_customer, _validLoanApp);
        }

        [TestCleanup]
        public void CleanUpService()
        {
            _service.Dispose();
        }

        [TestMethod]
        public void ProcessEndOfMonth()
        {
            _service.SetCurrentDate(new DateTime(2013, 11, 30, 15, 0, 0));
            var nextDay = _service.ProcessEndOfDay(); // should call process end of month because next day is the December, 1
            Assert.AreEqual(new DateTime(2013, 12, 1, 15, 0, 0), nextDay);
            Assert.AreEqual(1, _loan.Accounts.First(a => a.Type == AccountType.Interest).Entries.Count);
        }

        [TestMethod]
        public void CreateLoanContract()
        {
            // TODO: add data!
            var loan = _service.CreateLoanContract(_customer, new LoanApplication
            {
                CellPhone = "+37529-CREATE-LOAN-CONTRACT",
                Documents = new Collection<Document> { _passport },
                LoanAmount = 1000000,
                LoanPurpose = LoanPurpose.Common,
                Tariff = _tariff,
                Term = 3,
                TimeCreated = DateTime.Now,
            });
            Assert.IsNotNull(loan);
            // TODO: add check of accounts and so on
        }


        [TestMethod]
        public void ProcessEndOfDay()
        {
            _service.RegisterPayment(_loan, 1.0E4M);
            _service.ProcessEndOfDay();
            var contractServiceAcc = _loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.ContractService);

            Assert.IsNotNull(contractServiceAcc);
            Assert.AreEqual(0M, contractServiceAcc.Balance);
        }

        [TestMethod]
        public void RegisterPayment()
        {
            const decimal amount = 1.0E7M;
            var entry = _service.RegisterPayment(_loan, amount);
            Assert.AreEqual(amount, entry.Amount);
            var lastAddedEntry = _loan.Accounts.First(acc => acc.Type == AccountType.ContractService).Entries.Last();
            Assert.AreEqual(amount, lastAddedEntry.Amount);
        }

        [TestMethod]
        public void CloseLoanContract()
        {
            var canBeClosed = _loan.Accounts.All(acc => acc.Balance == 0M);
            if (canBeClosed)
            {
                Assert.IsTrue(_service.CloseLoanContract(_loan));
            }
            else
            {
                Assert.IsFalse(_service.CloseLoanContract(_loan));
            }
        }
    }
}

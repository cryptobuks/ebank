using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Application.Tests
{
    [TestClass]
    public class ProcessingServiceTest
    {
        private static UnityContainer _container;
        private static ProcessingService _service;
        private static Loan _loan;

        [ClassInitialize]
        public static void  InitService(TestContext context)
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
            _service = _container.Resolve<ProcessingService>();

            var customer = new Customer
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
            var passport = new Document
            {
                CustomerId = customer.Id,
                Customer = customer,
                DocType = DocType.Passport,
                TariffDocType = TariffDocType.DebtorPrimary,
                Number = "MP2345678"
            };
            var tariff = new Tariff
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
            //_service.SaveOrUpdateTariff(tariff);
            var validLoanApp = new LoanApplication
            {
                CellPhone = "+375291000000",
                Documents = new Collection<Document> { passport },
                LoanAmount = 5.5E7M,
                LoanPurpose = LoanPurpose.Common,
                Tariff = tariff,
                TariffId = tariff.Id,
                Term = 3,
                TimeCreated = DateTime.Now
            };
            _loan = _service.CreateLoanContract(validLoanApp);
        }

        [TestMethod]
        public void ProcessEndOfMonth()
        {
            _service.SetCurrentDateTime(new DateTime(2013, 11, 1, 15, 0, 0));
            _service.ProcessEndOfMonth(DateTime.UtcNow);
            Assert.AreEqual(1, _loan.Accounts.First(a => a.Type == AccountType.Interest).Entries.Count);
        }

        [TestMethod]
        public void CreateLoanContract()
        {
            // TODO: add data!
            var loan = _service.CreateLoanContract(new LoanApplication
            {
                CellPhone = String.Empty,
                Documents = null,
                LoanAmount = 1000000,
            });
            Assert.IsNotNull(loan);
            // TODO: add check of accounts and so on
        }


        [TestMethod]
        public void ProcessEndOfDay()
        {
            
            //var entry = new Entry()
            //    {
            //        Amount = 1.0E4M, 
            //        Currency = _loan.Application.Currency,
            //        Date = DateTime.Now,
            //        Id = Guid.NewGuid(),
            //        SubType = EntrySubType.ContractService, 
            //        Type = EntryType.Accrual
            //    };

            //var contractServiceAcc = _loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.ContractService);
            //if (contractServiceAcc == null) return;
            
            //contractServiceAcc.Entries.Add(entry);
            //var interestAccount = _loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.Interest);
            //var generalDebtAccount = _loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.GeneralDebt);
            
            //var contractAccountcAmount = contractServiceAcc.Balance;
            //if (interestAccount != null)
            //{
            //    var interestAccountAmount = interestAccount.Balance;
            //    var payment = Math.Min(contractAccountcAmount, interestAccountAmount);
            //}
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

using System.Collections.Generic;
using Application.LoanProcessing;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Application.Tests
{
    [TestClass]
    public class ProcessingServiceTest
    {
        [Dependency]
        protected ProcessingService Service { get; set; }

        private static Loan _loan;
        private static LoanApplication _validLoanApp;
        private static Tariff _tariff;
        private static PersonalData _passport;
        private static Customer _customer;

        //[ClassInitialize]
        //public static void InitService(TestContext context)
        //{
        //    Service.SetCurrentDate(new DateTime(2013, 12, 07));

        //    var bankAccountByr = Service.CreateAccount(Currency.BYR, AccountType.BankBalance);
        //    Service.AddEntry(bankAccountByr, new Entry()
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
        //        Passport = "MP2345678",
        //        LastName = "Mitchell",
        //        FirstName = "Stanley",
        //        MiddleName = "Matthew",
        //        DateOfBirth = new DateTime(1972, 10, 17),
        //        Identification = "317041972A0PB1",
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
        //        MinTerm = 1,
        //        MinAmount = 1.0E6M,
        //        Name = "NeverSeeMeAgain",
        //        PmtFrequency = 1,
        //        PmtType = PaymentCalculationType.Annuity
        //    };
        //    _validLoanApp = new LoanApplication
        //    {
        //        CellPhone = "+375291000000",
        //        Currency = _tariff.Currency,
        //        LoanAmount = 5.5E7M,
        //        LoanPurpose = LoanPurpose.Common,
        //        Tariff = _tariff,
        //        Term = 2,
        //        TimeCreated = DateTime.Now
        //    };
        //    _loan = Service.CreateLoanContract(_customer, _validLoanApp);
        //}

        [TestMethod]
        public void ProcessEndOfMonth()
        {
            Service.SetCurrentDate(new DateTime(2013, 11, 30, 15, 0, 0));
            var nextDay = Service.ProcessEndOfDay(); // should call process end of month because next day is the December, 1
            Assert.AreEqual(new DateTime(2013, 12, 1, 15, 0, 0), nextDay);
            Assert.AreEqual(1, _loan.Accounts.First(a => a.Type == AccountType.Interest).Entries.Count);
        }

        [TestMethod]
        public void CreateLoanContract()
        {
            var loan = Service.CreateLoanContract(_customer, new LoanApplication
            {
                CellPhone = "+37529-CREATE-LOAN-CONTRACT",
                Currency = _tariff.Currency,
                //Documents = new Collection<PersonalData> { _passport },
                LoanAmount = 1000000,
                LoanPurpose = LoanPurpose.Common,
                Tariff = _tariff,
                Term = 3,
                TimeCreated = DateTime.Now,
            });
            Assert.IsNotNull(loan);
            Assert.IsNotNull(loan.PaymentSchedule);
            Assert.AreEqual(3, loan.PaymentSchedule.Payments.Count);
        }

        [TestMethod]
        public void ProcessEndOfDay()
        {
            Service.SetCurrentDate(new DateTime(2013, 11, 29, 15, 0, 0));
            Service.RegisterPayment(_loan, 1.0E4M);
            Service.ProcessEndOfDay();
            var contractServiceAcc = _loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.ContractService);

            Assert.IsNotNull(contractServiceAcc);
            Assert.AreEqual(0M, contractServiceAcc.Balance);
        }

        [TestMethod]
        public void RegisterPayment()
        {
            const decimal amount = 1.0E7M;
            var entry = Service.RegisterPayment(_loan, amount);
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
                Assert.IsTrue(Service.CloseLoanContract(_loan));
            }
            else
            {
                Assert.IsFalse(Service.CloseLoanContract(_loan));
            }
        }

        [TestMethod]
        public void CalculateAnnuityPaymentSchedule()
        {
            _loan.Application.TimeContracted = new DateTime(2013, 12, 19, 14, 0, 0);
            var schedule = PaymentScheduleCalculator.CalculateAnnuitySchedule(_tariff, _loan.Application.LoanAmount,
                _loan.Application.Term, _loan.Application.TimeContracted);
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(schedule.Payments);
            Assert.AreEqual(_loan.Application.Term, schedule.Payments.Count);
            var delta = schedule.MainDebtOverallAmount - _loan.Application.LoanAmount;
            Assert.IsTrue(delta >= 0M);
            Assert.IsTrue(delta < 0.02M);
        }

        [TestMethod]
        public void CalculateStandardPaymentSchedule()
        {
            var pmtType = _tariff.PmtType;
            _tariff.PmtType = PaymentCalculationType.Standard;
            var schedule = PaymentScheduleCalculator.CalculateAnnuitySchedule(_tariff, _loan.Application.LoanAmount,
                _loan.Application.Term, _loan.Application.TimeContracted);
            _tariff.PmtType = pmtType;
            Assert.IsNotNull(schedule);
            Assert.IsNotNull(schedule.Payments);
            Assert.AreEqual(_loan.Application.Term, schedule.Payments.Count);
            var delta = schedule.MainDebtOverallAmount - _loan.Application.LoanAmount;
            Assert.IsTrue(delta >= 0M);
            Assert.IsTrue(delta < 0.02M);
        }
    }
}

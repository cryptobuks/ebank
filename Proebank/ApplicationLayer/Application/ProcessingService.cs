﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Application.LoanProcessing;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Repositories;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Domain;

namespace Application
{
    public class ProcessingService// : IDisposable
    {
        private readonly IUnitOfWork _unitOfWork;
        //private bool _disposed;
        private static bool DaySync;    //TODO: make flag in db
        private static bool MonthSync;  //TODO: make flag in db

        private static readonly AccountType[] LoanAccountTypes =
        {
            AccountType.ContractService,
            AccountType.GeneralDebt,
            AccountType.Interest,
            AccountType.OverdueGeneralDebt, 
            AccountType.OverdueInterest
        };

        public ProcessingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Process to end every banking day
        /// </summary>
        public DateTime ProcessEndOfDay()
        {
            if (!DaySync)
            {
                DaySync = true;
                var date = GetCurrentDate();
                ProcessContractServiceAccounts();
                var endOfMonth = DateTime.DaysInMonth(date.Year, date.Month) >= 30
                    ? 30
                    : DateTime.DaysInMonth(date.Year, date.Month);
                if (date.Day == endOfMonth)
                {
                    ProcessEndOfMonth(date);
                }
                DaySync = false;
                return MoveTime(1);
            }
            return GetCurrentDate();
        }

        private void ProcessContractServiceAccounts()
        {
            var today = GetCurrentDate();
            var loansWithMoneyOnServiceAccount = GetLoans().ToList();   // TODO: potentially OutOfMemory
            foreach (var loan in loansWithMoneyOnServiceAccount)
            {
                var accounts = loan.Accounts;
                var contractServiceAcc = loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.ContractService);
                var interestAccount = accounts.Single(acc => acc.Type == AccountType.Interest);
                var generalDebtAccount = accounts.Single(acc => acc.Type == AccountType.GeneralDebt);
                var overdueGeneralDebtAccount = accounts.Single(acc => acc.Type == AccountType.OverdueGeneralDebt);
                var overdueInterestAccount = accounts.Single(acc => acc.Type == AccountType.OverdueInterest);
                var repo = _unitOfWork.GetDbSet<Entry>();
                // We filter only loans with positive balance on contract service account
                if (contractServiceAcc != null && contractServiceAcc.Balance > 0)
                {
                    var amount = contractServiceAcc.Balance;
                    if (amount > 0M)
                    {
                        // at first we transfer money to interest account
                        var interestPayment = Math.Min(amount, interestAccount.Balance);
                        if (interestPayment > 0M)
                        {
                            var interestEntryPlus = repo.Create();
                            interestEntryPlus.Amount = interestPayment;
                            interestEntryPlus.Currency = loan.Application.Currency;
                            interestEntryPlus.Date = today;
                            interestEntryPlus.Type = EntryType.Payment;
                            interestEntryPlus.SubType = EntrySubType.Interest;
                            var interestEntryMinus = repo.Create();
                            Entry.GetOppositeFor(interestEntryPlus, interestEntryMinus);
                            AddEntry(interestAccount, interestEntryPlus);
                            AddEntry(contractServiceAcc, interestEntryMinus);
                            amount -= interestPayment;
                        }
                        // then to generalDebtAccount
                        var generalDebtPayment = Math.Min(amount, generalDebtAccount.Balance);
                        if (generalDebtPayment > 0M)
                        {
                            var generalDebtPlus = repo.Create();
                            generalDebtPlus.Amount = generalDebtPayment;
                            generalDebtPlus.Currency = loan.Application.Currency;
                            generalDebtPlus.Date = today;
                            generalDebtPlus.Type = EntryType.Payment;
                            generalDebtPlus.SubType = EntrySubType.GeneralDebt;
                            var generalDebtMinus = repo.Create();
                            Entry.GetOppositeFor(generalDebtPlus, generalDebtMinus);
                            AddEntry(generalDebtAccount, generalDebtPlus);
                            AddEntry(contractServiceAcc, generalDebtMinus);
                            amount -= generalDebtPayment;
                        }
                        var overdueInterestPayment = Math.Min(amount, overdueInterestAccount.Balance);
                        if (overdueInterestPayment > 0M)
                        {
                            var overdueInterestPlus = repo.Create();
                            overdueInterestPlus.Amount = overdueInterestPayment;
                            overdueInterestPlus.Currency = loan.Application.Currency;
                            overdueInterestPlus.Date = today;
                            overdueInterestPlus.Type = EntryType.Payment;
                            overdueInterestPlus.SubType = EntrySubType.Fine;
                            var overdueInterestMinus = repo.Create();
                            Entry.GetOppositeFor(overdueInterestPlus, overdueInterestMinus);
                            AddEntry(overdueInterestAccount, overdueInterestPlus);
                            AddEntry(contractServiceAcc, overdueInterestMinus);
                            amount -= overdueInterestPayment;
                        }
                    }
                }
            }
            UpdateDailyProcessingTime();
        }

        private void ProcessEndOfMonth(DateTime date)
        {
            if (!MonthSync)
            {
                MonthSync = true;
                var accruals = LoanProcessEndOfMonth(date);
                foreach (var accrual in accruals)
                {
                    AddEntry(accrual.Key, accrual.Value);
                }
                UpdateMonthlyProcessingTime();
                MonthSync = false;
            }
            //
            //var schedule = loan.PaymentSchedule;
            //var pmt = schedule.Payments.SingleOrDefault(p =>
            //    p.AccruedOn.HasValue && p.AccruedOn.Value.Year == today.Year &&
            //    p.AccruedOn.Value.DayOfYear == today.DayOfYear);
            //if (pmt != null)
            //{
            //    // TODO: fix with daily interest parts
            //    var interestEntryPlus = repo.Create();
            //    interestEntryPlus.Amount = pmt.AccruedInterestAmount;
            //    interestEntryPlus.Currency = loan.Application.Currency;
            //    interestEntryPlus.Date = today;
            //    interestEntryPlus.Type = EntryType.Accrual;
            //    interestEntryPlus.SubType = EntrySubType.Interest;
            //    AddEntry(interestAccount, interestEntryPlus);
            //}
        }

        public Loan CreateLoanContract(Customer customer, LoanApplication application)
        {
            var today = GetCurrentDate();
            var bankAccount = GetBankAccount(application.Currency);
            var schedule = PaymentScheduleCalculator.Calculate(application);
            var accounts = new List<Account>(LoanAccountTypes
                .Select(accountType =>
                {
                    var account = _unitOfWork.GetDbSet<Account>().Create();
                    account.Currency = application.Currency;
                    account.Type = accountType;
                    account.DateOpened = today;
                    account.Number = CreateAccountNumber(accountType);
                    account.Entries = new Collection<Entry>();
                    return account;
                }));
            var generalDebtAcc = accounts.Single(a => a.Type == AccountType.GeneralDebt);
            var entryDate = GetCurrentDate();
            var initialEntry = _unitOfWork.GetDbSet<Entry>().Create();
            initialEntry.Amount = application.LoanAmount;
            initialEntry.Currency = application.Currency;
            initialEntry.Date = entryDate;
            initialEntry.Type = EntryType.Transfer;
            initialEntry.SubType = EntrySubType.GeneralDebt;
            application.Status = LoanApplicationStatus.Contracted;

            var bankEntry = _unitOfWork.GetDbSet<Entry>().Create();
            Entry.GetOppositeFor(initialEntry, bankEntry);
            bankEntry.Type = EntryType.Transfer;
            bankEntry.SubType = EntrySubType.BankLoanIssued;
            AddEntry(generalDebtAcc, initialEntry);
            AddEntry(bankAccount, bankEntry);

            var loan = _unitOfWork.GetDbSet<Loan>().Create();
            loan.CustomerId = customer.Id;
            loan.Application = application;
            loan.Application.TimeContracted = GetCurrentDate();
            loan.IsClosed = false;
            loan.PaymentSchedule = schedule;
            loan.Accounts = accounts;
            UpsertLoan(loan);
            return loan;
        }

        private int CreateAccountNumber(AccountType accountType)
        {
            var accRepo = _unitOfWork.GetDbSet<Account>();
            var accounts = accRepo.Where(acc => acc.Type == accountType);
            var currentMax = accounts.Any() ? accounts.Max(a => a.Number) : -1;
            return currentMax + 1;
        }

        private Account GetBankAccount(Currency currency)
        {
            var accountRepo = _unitOfWork.GetDbSet<Account>();
            return accountRepo.Single(acc => acc.Type == AccountType.BankBalance &&  acc.Currency == currency);
        }

        /// <summary>
        /// Registers monthly payment. Basically, it just transfers money to contract service loan account (3819)
        /// </summary>
        /// <param name="loan"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public Entry RegisterPayment(Loan loan, decimal amount)
        {
            var accounts = loan.Accounts;
            var contractAccount = accounts.First(a => a.Type == AccountType.ContractService);
            var entrySet = _unitOfWork.GetDbSet<Entry>();
            var entry = entrySet.Create();
            entry.Amount = amount;
            entry.Currency = loan.Application.Currency;
            entry.Date = GetCurrentDate();
            entry.Type = EntryType.Payment;
            entry.SubType = EntrySubType.ContractService;
            AddEntry(contractAccount, entry);
            return entry;
        }

        public bool CloseLoanContract(Loan loan)
        {
            var canBeClosed = CanLoanBeClosed(loan);
            if (canBeClosed)
            {
                foreach (var account in loan.Accounts)
                {
                    CloseAccount(account);
                }
                CloseLoan(loan);
            }
            return canBeClosed;
        }

        private Dictionary<Account, Entry> LoanProcessEndOfMonth(DateTime currentDate)
        {
            var loanRepository = _unitOfWork.GetDbSet<Loan>();
            var entryRepository = _unitOfWork.GetDbSet<Entry>();
            return loanRepository
                .Where(l => !l.IsClosed)
                .ToList()
                .ToDictionary(
                    loan => loan.Accounts.Single(acc => acc.Type == AccountType.Interest),
                    loan =>
                    {
                        var entry = entryRepository.Create();
                        InterestCalculator.CalculateInterestFor(loan, currentDate, entry);
                        return entry;
                    });
        }

        private Dictionary<Account, Entry> LoanProcessEndOfMonthFines(DateTime currentDate)
        {
            var loanRepository = _unitOfWork.GetDbSet<Loan>();
            var entryRepository = _unitOfWork.GetDbSet<Entry>();
            return loanRepository
                .Where(l => !l.IsClosed)
                .ToList()
                .ToDictionary(
                    loan => loan.Accounts.Single(acc => acc.Type == AccountType.OverdueInterest),
                    loan =>
                    {
                        var entry = entryRepository.Create();
                        InterestCalculator.CalculateInterestFor(loan, currentDate, entry);
                        return entry;
                    });
        }

        public T Find<T>(Guid? id) where T : Entity
        {
            var repository = _unitOfWork.GetDbSet<T>();
            return repository.Find(id);
        }

        #region Loan service methods
        public IQueryable<Loan> GetLoans()
        {
            var loanRepository = _unitOfWork.GetDbSet<Loan>();
            return loanRepository;
        }

        private void UpsertLoan(Loan loan)
        {
            var loanRepo = _unitOfWork.GetDbSet<Loan>();
            loanRepo.AddOrUpdate(loan);
            _unitOfWork.SaveChanges();
        }

        private bool CanLoanBeClosed(Loan loan)
        {
            return loan.Accounts.All(a => a.Balance == 0M);
        }

        private void CloseLoan(Loan loan)
        {
            var loanRepo = _unitOfWork.GetDbSet<Loan>();
            loan.IsClosed = true;
            loanRepo.AddOrUpdate(loan);
            _unitOfWork.SaveChanges();
        }
	    #endregion

        #region Loan application service methods
        public IQueryable<LoanApplication> GetLoanApplications(bool showRemoved = false)
        {
            var loanApplicationRepo = _unitOfWork.GetDbSet<LoanApplication>();
            return loanApplicationRepo.Where(la => showRemoved || !la.IsRemoved);
        }

        public void UpsertLoanApplication(LoanApplication loanApplication)
        {
            var loanApplicationRepo = _unitOfWork.GetDbSet<LoanApplication>();
            loanApplicationRepo.AddOrUpdate(loanApplication);
            _unitOfWork.SaveChanges();
        }

        public void DeleteLoanApplicationById(Guid id)
        {
            var loanApplicationRepo = _unitOfWork.GetDbSet<LoanApplication>();
            var loanApplication = loanApplicationRepo.Find(id);
            loanApplicationRepo.Remove(loanApplication);
            _unitOfWork.SaveChanges();
        }

        public void CreateLoanApplication(LoanApplication loanApplication, bool fromConsultant = false)
        {
            loanApplication.TimeCreated = GetCurrentDate();
            loanApplication.Status = fromConsultant ? LoanApplicationStatus.Filled : LoanApplicationStatus.New;
            var selectedTariff = GetTariffs().Single(t => t.Id.Equals(loanApplication.TariffId));
            loanApplication.Tariff = selectedTariff;
            loanApplication.LoanPurpose = selectedTariff.LoanPurpose;
            loanApplication.Currency = selectedTariff.Currency;

            var loanApplicationRepo = _unitOfWork.GetDbSet<LoanApplication>();
            var validationResult = ValidateLoanApplication(loanApplication);
            if (validationResult.Count == 0)
            {
                loanApplicationRepo.AddOrUpdate(loanApplication);
                _unitOfWork.SaveChanges();
            }
            else
            {
                var ex = new ArgumentException("Loan application is not valid");
                ex.Data["validationResult"] = validationResult;
                throw ex;
            } 
        }

        public void ApproveLoanAppication(LoanApplication loanApplication)
        {
            var loanRepo = _unitOfWork.GetDbSet<LoanApplication>();
            loanApplication.Status = LoanApplicationStatus.Approved;
            loanRepo.AddOrUpdate(loanApplication);
            _unitOfWork.SaveChanges();
        }

        public void RejectLoanApplication(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.Rejected;
            var loanRepository = _unitOfWork.GetDbSet<LoanApplication>();
            loanRepository.AddOrUpdate(loanApplication);
            _unitOfWork.SaveChanges();
        }

        public void SendLoanApplicationToCommittee(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.UnderCommitteeConsideration;
            var loanRepository = _unitOfWork.GetDbSet<LoanApplication>();
            loanRepository.AddOrUpdate(loanApplication);
            _unitOfWork.SaveChanges();
        }

        public void SendLoanApplicationToSecurity(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.UnderRiskConsideration;
            var loanRepository = _unitOfWork.GetDbSet<LoanApplication>();
            loanRepository.AddOrUpdate(loanApplication);
            _unitOfWork.SaveChanges();
        }
        #endregion

        #region Tariff service methods
        public IQueryable<Tariff> GetTariffs()
        {
            var tariffRepo = _unitOfWork.GetDbSet<Tariff>();
            return tariffRepo;
        }

        public void UpsertTariff(Tariff tariff)
        {
            var tariffRepo = _unitOfWork.GetDbSet<Tariff>();
            tariffRepo.AddOrUpdate(tariff);
            _unitOfWork.SaveChanges();
        }

        public void DeleteTariffById(Guid id)
        {
            var tariffRepo = _unitOfWork.GetDbSet<Tariff>();
            var tariff = tariffRepo.Find(id);
            tariff.IsActive = false;
            tariffRepo.Remove(tariff);
            _unitOfWork.SaveChanges();
        }

        private Dictionary<string, string> ValidateLoanApplication(LoanApplication loanApplication)
        {
            //if (loanApplication == null || loanApplication.Tariff == null)
            if (loanApplication == null || loanApplication.TariffId.Equals(Guid.Empty))
            {
                throw new ArgumentException("loanApplication");
            }
            var validationResult = new Dictionary<string, string>();
            var isEnoughMoney = GetBankAccount(loanApplication.Currency).Balance >= loanApplication.LoanAmount;
            var tariff = _unitOfWork.GetDbSet<Tariff>().Find(loanApplication.TariffId);
            var amount = loanApplication.LoanAmount;
            var term = loanApplication.Term;
            var isAmountValid = amount >= tariff.MinAmount && amount <= tariff.MaxAmount;
            var isTermValid = term >= tariff.MinTerm && term <= tariff.MaxTerm;
            if (!isEnoughMoney)
            {
                validationResult.Add("LoanAmount", "Not enough money");
            }
            if (!isAmountValid)
            {
                validationResult.Add("LoanAmount", "Loan amount is not correct");
            }
            if (!isTermValid)
            {
                validationResult.Add("Term","Term is not valid");
            }
            return validationResult;
        }

        #endregion

        #region BankCalendar methods
        private DateTime MoveTime(byte days)
        {
            var currentCalendar = GetCalendar(true);
            Debug.Assert(currentCalendar != null, "currentCalendar != null");
            Debug.Assert(currentCalendar.CurrentTime != null, "currentCalendar.CurrentTime != null");
            var result = currentCalendar.CurrentTime.Value + new TimeSpan(1, 0, 0, 0);
            currentCalendar.CurrentTime = result;
            currentCalendar.ProcessingLock = false;
            _unitOfWork.SaveChanges();
            return result;
        }

        private Calendar GetCalendar(bool withLock = false)
        {
            var calendarRepository = _unitOfWork.GetDbSet<Calendar>();
            var calendar = calendarRepository.Single();
            if (withLock)
            {
                if (!calendar.ProcessingLock)
                {
                    calendar.ProcessingLock = true;
                    calendarRepository.AddOrUpdate(calendar);
                    _unitOfWork.SaveChanges();
                }
                else
                {
                    throw new Exception("Calendar is locked");
                }
            }
            return calendar;
        }

        private void UpdateDailyProcessingTime()
        {
            var calendarRepository = _unitOfWork.GetDbSet<Calendar>();
            var currentCalendar = calendarRepository.Single();
            currentCalendar.LastDailyProcessingTime = currentCalendar.CurrentTime;
            calendarRepository.AddOrUpdate(currentCalendar);
            _unitOfWork.SaveChanges();
        }

        private void UpdateMonthlyProcessingTime()
        {
            var calendarRepo = _unitOfWork.GetDbSet<Calendar>();
            var currentCalendar = calendarRepo.Single();
            currentCalendar.LastMonthlyProcessingTime = currentCalendar.CurrentTime;
            calendarRepo.AddOrUpdate(currentCalendar);
            _unitOfWork.SaveChanges();
        }

        public DateTime GetCurrentDate()
        {
            var calendarRepo = _unitOfWork.GetDbSet<Calendar>();
            var calendar = calendarRepo.SingleOrDefault();
            if (calendar != null)
            {
                Debug.Assert(calendar.CurrentTime != null, "calendar.CurrentTime != null");
                return calendar.CurrentTime.Value;
            }
            calendar = new Calendar
            {
                Id = Calendar.ConstGuid,
                CurrentTime = DateTime.UtcNow
            };
            calendarRepo.AddOrUpdate(calendar);
            Debug.Assert(calendar.CurrentTime != null, "calendar.CurrentTime != null");
            return calendar.CurrentTime.Value;
        }

        public void SetCurrentDate(DateTime dateTime)
        {
            var calendarRepo = _unitOfWork.GetDbSet<Calendar>();
            if (calendarRepo.Any())
            {
                calendarRepo.First().CurrentTime = dateTime;
            }
            else
            {
                var calendar = new Calendar
                {
                    Id = Calendar.ConstGuid,
                    CurrentTime = dateTime
                };
                calendarRepo.AddOrUpdate(calendar);
            }
            _unitOfWork.SaveChanges();
        }
        #endregion

        #region Account service
        public Account CreateAccount(Currency currency, AccountType accountType)
        {
            var accountRepo = _unitOfWork.GetDbSet<Account>();
            var accountWithSameType = accountRepo.Where(a => a.Type.Equals(accountType));
            var nextNumber = accountWithSameType.Any()
                ? accountWithSameType.Max(a => a.Number) + 1
                : 1;
            var acc = new Account
            {
                Currency = currency,
                DateOpened = GetCurrentDate(),
                Entries = new Collection<Entry>(),
                Number = nextNumber,
                Type = accountType,
            };
            accountRepo.AddOrUpdate(acc);
            _unitOfWork.SaveChanges();
            return acc;
        }

        public void AddEntry(Account account, Entry entry)
        {
            var accountRepo = _unitOfWork.GetDbSet<Account>();
            if (account == null)
                throw new ArgumentNullException("account");
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (account.Currency != entry.Currency)
                throw new ArgumentException("Currencies are not equal");
            account.Entries.Add(entry);
            accountRepo.AddOrUpdate(account);
            _unitOfWork.SaveChanges();
        }

        public void CloseAccount(Account account)
        {
            var accountRepo = _unitOfWork.GetDbSet<Account>();
            if (account.IsClosed)
            {
                throw new ArgumentException("Account is already closed");
            }
            account.IsClosed = true;
            account.DateClosed = GetCurrentDate();
            accountRepo.AddOrUpdate(account);
            // TODO: check saving
        }
        #endregion

        public IEnumerable<LoanHistory> GetHistoryFromNationalBank(LoanApplication application)
        {
            var nationalBank = _unitOfWork.GetDbSet<LoanHistory>();
            var personId = application.PersonalData.Identification;
            var history = nationalBank.Where(l => l.Person.Identification == personId).ToList();
            if (!history.Any())
            {
                var gen = new Random();

                foreach (var i in Enumerable.Range(1, gen.Next(2, 6)))
                {
                    var started = new DateTime(2013 - gen.Next(0, 5), gen.Next(1, 12), gen.Next(1, 25));
                    var closed = started.AddMonths(gen.Next(3, 60));
                    var isClosed = closed <= GetCurrentDate();
                    var histItem = new LoanHistory
                    {
                        Amount = gen.Next(1, 500)*10000,
                        Currency = Currency.BYR,
                        HadProblems = gen.NextDouble() > 0.85,
                        Person = application.PersonalData,
                        WhenOpened = started,
                        WhenClosed = isClosed ? closed : (DateTime?) null,
                    };
                    history.Add(histItem);
                    nationalBank.AddOrUpdate(histItem);
                }
                _unitOfWork.SaveChanges();
            }
            return history.OrderBy(l => l.WhenOpened);
        }

        //public void Dispose()
        //{
        //    Dispose(true);
        //}

        //private void Dispose(bool disposing)
        //{
        //    if (!_disposed)
        //    {
        //        if (disposing)
        //        {
        //            //_unitOfWork.Dispose();
        //        }
        //        _disposed = true;
        //    }
        //}
    }
}

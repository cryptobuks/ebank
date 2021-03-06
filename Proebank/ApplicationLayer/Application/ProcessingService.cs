﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Diagnostics;
using System.Linq;
using Application.LoanProcessing;
using Domain.Enums;
using Domain.Models;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain;

namespace Application
{
    public class ProcessingService
    {
        private readonly IUnitOfWork _unitOfWork;
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
                try
                {
                    DaySync = true;
                    var date = GetCurrentDate();
                    date = date.AddHours(10);
                    SetCurrentDate(date);

                    // At first we transfer money from contract service accounts
                    ProcessContractServiceAccounts();
                    MarkPaymentsAsPaid();
                    date = date.AddHours(1);
                    SetCurrentDate(date);

                    // Next we accrue fines (daily)
                    AccrueDailyFines();
                    date = date.AddHours(1);
                    SetCurrentDate(date);

                    // Then we accrue planned interest
                    ProcessPlannedInterestAccruals(date);
                    date = date.AddHours(1);
                    SetCurrentDate(date);

                    // Finally, we process overdue
                    ProcessUnpaidPayments(date);

                    SetCurrentDate(date.AddHours(11));  // instead of MoveNextDay() call to have more realistic processing times
                    _unitOfWork.SaveChanges();
                }
                finally
                {
                    DaySync = false;
                }
            }
            return GetCurrentDate();
        }

        private void MarkPaymentsAsPaid()
        {
            var today = GetCurrentDate();
            var loans = GetLoans().Where(l => !l.IsClosed).ToList();
            foreach (var loan in loans)
            {
                var mainDebtBalance = loan.Accounts.Single(acc => acc.Type == AccountType.GeneralDebt).Balance;
                var interestBalance = loan.Accounts.Single(acc => acc.Type == AccountType.Interest).Balance;
                var overdueInterestBalance = loan.Accounts.Single(acc => acc.Type == AccountType.OverdueInterest).Balance;
                if (interestBalance == 0M && overdueInterestBalance == 0M)
                {
                    var pmtRepo = _unitOfWork.GetDbSet<Payment>();
                    var pmts = loan.PaymentSchedule.Payments;
                    var unpaidPmts = pmts.Where(p => !p.IsPaid).OrderBy(pmt => pmt.AccruedOn).ToArray();
                    var mainDebtSums = new List<decimal>();
                    for (var i = 0; i < unpaidPmts.Length; i++)
                    {
                        for (var j = 0; j < mainDebtSums.Count; j++)
                        {
                            mainDebtSums[j] += unpaidPmts[i].MainDebtAmount;
                        }
                        mainDebtSums.Add(unpaidPmts[i].MainDebtAmount);
                    }
                    mainDebtSums.Add(0M);
                    mainDebtSums = mainDebtSums.Skip(1).ToList();
                    for (var i = 0; i < mainDebtSums.Count && mainDebtBalance <= mainDebtSums[i]; i++)
                    {
                        unpaidPmts[i].IsPaid = true;
                        pmtRepo.AddOrUpdate(unpaidPmts[i]);
                    }
                }
            }
        }

        private void AccrueDailyFines()
        {
            var today = GetCurrentDate();
            var entrySet = _unitOfWork.GetDbSet<Entry>();
            var loansWithMoneyOnServiceAccount = GetLoans().ToList();
            foreach (var loan in loansWithMoneyOnServiceAccount)
            {
                var mainDebtAccount = loan.Accounts.Single(acc => acc.Type == AccountType.GeneralDebt);
                var overdueInterestAccount = loan.Accounts.Single(acc => acc.Type == AccountType.OverdueInterest);
                var balance = overdueInterestAccount.Balance;
                if (balance > 0M)
                {
                    var fineEntry = entrySet.Create();
                    var preciseAmount = (mainDebtAccount.Balance + balance) * loan.Application.Tariff.InterestRate / 180;   // 360/2 - interest rate in doubled
                    var fineAmount = Math.Ceiling(preciseAmount * 100) / 100;  
                    FillEntryValues(fineEntry, fineAmount, loan, today, EntryType.Accrual, EntrySubType.Fine);
                    AddEntry(overdueInterestAccount, fineEntry);
                }
            }
        }

        private void ProcessContractServiceAccounts()
        {
            var today = GetCurrentDate();
            var loansWithMoneyOnServiceAccount = GetLoans().ToList();
            foreach (var loan in loansWithMoneyOnServiceAccount)
            {
                var accounts = loan.Accounts;
                var contractServiceAcc = loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.ContractService);
                var bankAccount = GetBankAccount(loan.Application.Currency);
                var interestAccount = accounts.Single(acc => acc.Type == AccountType.Interest);
                var generalDebtAccount = accounts.Single(acc => acc.Type == AccountType.GeneralDebt);
                var overdueInterestAccount = accounts.Single(acc => acc.Type == AccountType.OverdueInterest);
                var repo = _unitOfWork.GetDbSet<Entry>();
                // We filter only loans with positive balance on contract service account
                if (contractServiceAcc != null && contractServiceAcc.Balance > 0)
                {
                    var startAmount = contractServiceAcc.Balance;
                    var amount = startAmount;
                    if (amount > 0M)
                    {
                        // at first we repay overdue interest
                        amount = RepayOverdueInterest(amount, overdueInterestAccount, repo, loan, today);
                        // then we transfer money to interest account
                        amount = RepayInterest(amount, interestAccount, repo, loan, today);
                        // finally to generalDebtAccount
                        amount = RepayMainDebt(amount, generalDebtAccount, repo, loan, today);

                        // Here we take care of the only "active" account - 3819
                        if (startAmount - amount > 0M)
                        {
                            var bankDebit = repo.Create();
                            var contractCredit = repo.Create();
                            FillEntryValues(bankDebit, startAmount - amount, loan, today, EntryType.Transfer, EntrySubType.BankLoanFromContract);
                            FillEntryValues(contractCredit, amount - startAmount, loan, today, EntryType.Transfer, EntrySubType.BankLoanFromContract);
                            AddEntry(bankAccount, bankDebit);
                            AddEntry(contractServiceAcc, contractCredit);
                        }
                    }
                }
            }
            UpdateDailyProcessingTime();
        }

        private decimal RepayOverdueInterest(decimal amount, Account overdueInterestAccount, IDbSet<Entry> repo, Loan loan,
            DateTime today)
        {
            var overdueInterestPmt = Math.Min(amount, overdueInterestAccount.Balance);
            if (overdueInterestPmt > 0M)
            {
                var overdueInterestPlus = repo.Create();
                FillEntryValues(overdueInterestPlus, -overdueInterestPmt, loan, today, EntryType.Payment, EntrySubType.Fine);
                AddEntry(overdueInterestAccount, overdueInterestPlus);
                amount -= overdueInterestPmt;
            }
            return amount;
        }

        private decimal RepayMainDebt(decimal amount, Account generalDebtAccount, IDbSet<Entry> repo, Loan loan, DateTime today)
        {
            var generalDebtPmt = Math.Min(amount, generalDebtAccount.Balance);
            if (generalDebtPmt > 0M)
            {
                var generalDebtPlus = repo.Create();
                FillEntryValues(generalDebtPlus, -generalDebtPmt, loan, today, EntryType.Payment, EntrySubType.GeneralDebt);
                AddEntry(generalDebtAccount, generalDebtPlus);
                amount -= generalDebtPmt;
            }
            return amount;
        }

        private decimal RepayInterest(decimal amount, Account interestAccount, IDbSet<Entry> repo, Loan loan, DateTime today)
        {
            var interestPmt = Math.Min(amount, interestAccount.Balance);
            if (interestPmt > 0M)
            {
                var interestEntryPlus = repo.Create();
                FillEntryValues(interestEntryPlus, -interestPmt, loan, today, EntryType.Payment, EntrySubType.Interest);
                AddEntry(interestAccount, interestEntryPlus);
                amount -= interestPmt;
            }
            return amount;
        }

        private static void FillEntryValues(Entry entry, decimal pmtAmount, Loan loan, DateTime date, EntryType entryType, EntrySubType entrySubtype)
        {
            entry.Amount = pmtAmount;
            entry.Currency = loan.Application.Currency;
            entry.Date = date;
            entry.Type = entryType;
            entry.SubType = entrySubtype;
        }

        private void ProcessPlannedInterestAccruals(DateTime date)
        {
            if (!MonthSync)
            {
                MonthSync = true;
                var accruals = LoanProcessEndOfMonth(date);
                foreach (var accrual in accruals)
                {
                    if (accrual.Value.Amount > 0)
                    {
                        AddEntry(accrual.Key, accrual.Value);
                    }
                }
                UpdateMonthlyProcessingTime();
                MonthSync = false;
            }
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

        private Dictionary<Account, Entry> LoanProcessEndOfMonth(DateTime currentDate)
        {
            var loanRepository = _unitOfWork.GetDbSet<Loan>();
            var entryRepository = _unitOfWork.GetDbSet<Entry>();
            var loansToProcess =  loanRepository
                .Where(l => !l.IsClosed)
                .ToList()
                // doesn't work with IQueryable , so we need explicit ToList() call
                .Where(l => l.PaymentSchedule.Payments
                    .Any(p => p.AccruedOn.HasValue && p.AccruedOn.Value.Date == currentDate.Date))
                .ToList();
            return loansToProcess.ToDictionary(
                    loan => loan.Accounts.Single(acc => acc.Type == AccountType.Interest),
                    loan =>
                    {
                        var entry = entryRepository.Create();
                        InterestCalculator.CalculateInterestFor(loan, currentDate, entry);
                        return entry;
                    });
        }

        /// <summary>
        /// Transfer remain funds from interest to overdue interest account
        /// </summary>
        /// <param name="currentDate">today date</param>
        private void ProcessUnpaidPayments(DateTime currentDate)
        {
            var loanSet = _unitOfWork.GetDbSet<Loan>();
            var entrySet = _unitOfWork.GetDbSet<Entry>();
            foreach (var loan in loanSet.ToList())
            {
                var payments = loan.PaymentSchedule.Payments;
                if (payments.Any(p => p.ShouldBePaidBefore.HasValue &&
                            p.ShouldBePaidBefore.Value.Date.AddDays(1) == currentDate.Date))
                {
                    var interestAccount = loan.Accounts.Single(acc => acc.Type == AccountType.Interest);
                    var unpaidInterest = interestAccount.Balance;
                    if (unpaidInterest > 0M)
                    {
                        var overdueInterestAccount = loan.Accounts.Single(acc => acc.Type == AccountType.OverdueInterest);
                        var entryMinus = entrySet.Create();
                        FillEntryValues(entryMinus, -unpaidInterest, loan, currentDate, EntryType.Transfer,
                            EntrySubType.Fine);
                        var entryPlus = entrySet.Create();
                        FillEntryValues(entryPlus, unpaidInterest, loan, currentDate, EntryType.Accrual,
                            EntrySubType.Fine);
                        AddEntry(interestAccount, entryMinus);
                        AddEntry(overdueInterestAccount, entryPlus);
                    }
                }
            }
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

        public Loan CreateLoanContract(Customer customer, LoanApplication application, string employeeId)
        {
            var today = GetCurrentDate();
            var bankAccount = GetBankAccount(application.Currency);
            var schedule = PaymentScheduleCalculator.Calculate(application);
            var accountsSet = _unitOfWork.GetDbSet<Account>();
            var entrySet = _unitOfWork.GetDbSet<Entry>();

            var accounts = new List<Account>(LoanAccountTypes
                .Select(accountType =>
                {
                    var account = accountsSet.Create();
                    account.Currency = application.Currency;
                    account.Type = accountType;
                    account.DateOpened = today;
                    account.Number = CreateAccountNumber(accountType);
                    account.Entries = new Collection<Entry>();
                    return account;
                }));
            var generalDebtAcc = accounts.Single(a => a.Type == AccountType.GeneralDebt);
            var entryDate = GetCurrentDate();
            var initialEntry = entrySet.Create();
            initialEntry.Amount = application.LoanAmount;
            initialEntry.Currency = application.Currency;
            initialEntry.Date = entryDate;
            initialEntry.Type = EntryType.Transfer;
            initialEntry.SubType = EntrySubType.GeneralDebt;
            application.Status = LoanApplicationStatus.ContractPrinted;

            var bankEntry = entrySet.Create();
            Entry.GetOppositeFor(initialEntry, bankEntry);
            bankEntry.Type = EntryType.Transfer;
            bankEntry.SubType = EntrySubType.BankLoanIssued;
            AddEntry(generalDebtAcc, initialEntry);
            AddEntry(bankAccount, bankEntry);

            var loan = new Loan
            {
                CustomerId = customer.Id,
                Application = application,
                IsClosed = false,
                PaymentSchedule = schedule,
                Accounts = accounts,
                IsContractSigned = false,
                EmployeeId = employeeId
            };
            loan.Application.TimeContracted = GetCurrentDate();
            // TODO: check if entries are saved!
            UpsertLoan(loan);
            return loan;
        }

        public void SignLoanContract(Guid laId)
        {
            var loanSet = _unitOfWork.GetDbSet<Loan>();
            var loan = loanSet.SingleOrDefault(l => l.Application.Id == laId);
            if (loan != null)
            {
                loan.IsContractSigned = true;
                loan.Application.Status = LoanApplicationStatus.Contracted;
                UpsertLoan(loan);
                var loanHistorySet = _unitOfWork.GetDbSet<LoanHistory>();
                loanHistorySet.Add(new LoanHistory(loan));
                _unitOfWork.SaveChanges();
            }
        }

        public bool CanLoanBeClosed(Loan loan)
        {
            return !loan.IsClosed && loan.Accounts
                .Where(acc => acc.Type != AccountType.ContractService)
                .All(a => a.Balance == 0M);
        }

        public void CloseLoan(Loan loan)
        {
            var loanRepo = _unitOfWork.GetDbSet<Loan>();
            var loanHistorySet = _unitOfWork.GetDbSet<LoanHistory>();
            var today = GetCurrentDate();
            if (CanLoanBeClosed(loan))
            {
                var entryRepo = _unitOfWork.GetDbSet<Entry>();
                loan.IsClosed = true;
                // ?? is for contracts signed before saving loan id in history entry
                var loanHistory = loanHistorySet.SingleOrDefault(lh => lh.LoanId == loan.Id) ??
                                  loanHistorySet.FirstOrDefault(
                                      lh =>
                                          lh.Amount == loan.Application.LoanAmount &&
                                          lh.Currency == loan.Application.Currency);
                if (loanHistory != null)
                {
                    loanHistory.HadProblems =
                        loan.Accounts.Single(a => a.Type == AccountType.OverdueInterest).Entries.Any();
                    loanHistory.WhenClosed = today;
                }
                foreach (var account in loan.Accounts)
                {
                    if (account.Type == AccountType.ContractService)
                    {
                        var balance = account.Balance;
                        if (balance > 0M)
                        {
                            var entry = entryRepo.Create();
                            FillEntryValues(entry, -balance, loan, GetCurrentDate(), EntryType.Transfer, EntrySubType.ContractService);
                            AddEntry(account, entry);
                        }
                    }
                    CloseAccount(account);
                }
                loanHistorySet.AddOrUpdate(loanHistory);
                loanRepo.AddOrUpdate(loan);
                _unitOfWork.SaveChanges();
            }
        }
	    #endregion

        #region Loan application service methods
        public IQueryable<LoanApplication> GetLoanApplications(bool showRemoved = false)
        {
            var loanApplicationRepo = _unitOfWork.GetDbSet<LoanApplication>();
            return loanApplicationRepo
                .Where(la => showRemoved || !la.IsRemoved)
                .OrderBy(la => la.TimeCreated);
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
            if (!selectedTariff.IsGuarantorNeeded)
            {
                loanApplication.Guarantor = null;
            }

            var loanApplicationRepo = _unitOfWork.GetDbSet<LoanApplication>();
            var validationResult = ValidateLoanApplication(loanApplication, fromConsultant);
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
            //tariffRepo.Remove(tariff);
            _unitOfWork.SaveChanges();
        }

        public Dictionary<string, string> ValidateLoanApplication(LoanApplication loanApplication, bool fromFill)
        {
            if (loanApplication == null || loanApplication.TariffId.Equals(Guid.Empty))
            {
                throw new ArgumentException("loanApplication");
            }
            var today = GetCurrentDate();
            var validationResult = new Dictionary<string, string>();
            var isEnoughMoney = GetBankAccount(loanApplication.Currency).Balance >= loanApplication.LoanAmount;
            var tariff = _unitOfWork.GetDbSet<Tariff>().Find(loanApplication.TariffId);
            var amount = loanApplication.LoanAmount;
            var term = loanApplication.Term;
            var isAmountValid = amount >= tariff.MinAmount && amount <= tariff.MaxAmount;
            var isTermValid = term >= tariff.MinTerm && term <= tariff.MaxTerm;
            var debtorData = loanApplication.PersonalData;
            var minAge = Math.Max(18, tariff.MinAge);
            var maxAge = Math.Min(65, tariff.MaxAge);
            var minValidDateOfBirth = today.Date.AddYears(-minAge);
            var maxValidDateOfBirth = today.Date.AddYears(-maxAge);
            if (!isEnoughMoney)
            {
                validationResult.Add("LoanAmount", "Not enough money - try later");
            }
            if (!isAmountValid)
            {
                validationResult.Add("LoanAmount", "Loan amount is not correct");
            }
            if (!isTermValid)
            {
                validationResult.Add("Term","Term is not valid");
            }
            if (fromFill)
            {
                if (debtorData == null)
                {
                    validationResult.Add("PersonalData", "Is not filled");
                }
                else
                {
                    if (debtorData.DateOfBirth.HasValue)
                    {
                        var dateOfBirth = debtorData.DateOfBirth.Value;
                        if (dateOfBirth > minValidDateOfBirth)
                        {
                            validationResult.Add("PersonalData", "Debtor must be at least " + minAge + " years old");
                        }
                        if (dateOfBirth < maxValidDateOfBirth)
                        {
                            validationResult.Add("PersonalData", "Debtor must be at most " + maxAge +" years old");
                        }
                        if (dateOfBirth > today.AddYears(-loanApplication.LengthOfWork))
                        {
                            validationResult.Add("LengthOfWork", "This value seems to be incorrect");
                        }
                    }
                    else
                    {
                        validationResult.Add("PersonalData", "Date of birth is not filled");
                    }
                }
            }
            if (fromFill && tariff.IsGuarantorNeeded)
            {
                var guarantorData = loanApplication.Guarantor;
                if (guarantorData == null)
                {
                    validationResult.Add("Guarantor", "Guarantor must be filled");
                }
                else
                {
                    if (guarantorData.DateOfBirth.HasValue)
                    {
                        var dateOfBirth = guarantorData.DateOfBirth.Value;
                        if (dateOfBirth > minValidDateOfBirth)
                        {
                            validationResult.Add("Guarantor", "Guarantor must be at least 18 years old");
                        }
                        if (dateOfBirth < maxValidDateOfBirth)
                        {
                            validationResult.Add("Guarantor", "Guarantor must be at most 65 years old");
                        }
                    }
                    else
                    {
                        validationResult.Add("Guarantor", "Guarantor date of birth is not filled");
                    }
                }
            }
            return validationResult;
        }

        #endregion

        #region BankCalendar methods
        private void MoveNextDay()
        {
            var currentCalendar = GetCalendar(true);
            Debug.Assert(currentCalendar != null, "currentCalendar != null");
            Debug.Assert(currentCalendar.CurrentTime != null, "currentCalendar.CurrentTime != null");
            var result = currentCalendar.CurrentTime.Value + new TimeSpan(1, 0, 0, 0);
            currentCalendar.CurrentTime = result;
            currentCalendar.ProcessingLock = false;
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
            currentCalendar.LastDailyProcessingDate = currentCalendar.CurrentTime;
            calendarRepository.AddOrUpdate(currentCalendar);
        }

        private void UpdateMonthlyProcessingTime()
        {
            var calendarRepo = _unitOfWork.GetDbSet<Calendar>();
            var currentCalendar = calendarRepo.Single();
            currentCalendar.LastMonthlyProcessingDate = currentCalendar.CurrentTime;
            currentCalendar.NextMonthlyProcessingDate = GetNextMonthProcessingDate();
            calendarRepo.AddOrUpdate(currentCalendar);
        }

        private DateTime GetNextMonthProcessingDate()
        {
            var today = GetCurrentDate();
            var endOfMonthDay = DateTime.DaysInMonth(today.Year, today.Month) >= 30
                ? 30
                : DateTime.DaysInMonth(today.Year, today.Month);
            var endOfMonthDate = new DateTime(today.Year, today.Month, endOfMonthDay);
            if (today.Date > endOfMonthDate) // 31 > 30
            {
                endOfMonthDate = endOfMonthDate.AddMonths(1);
                if (endOfMonthDate.Day == 31)
                {
                    endOfMonthDate = endOfMonthDate.AddDays(-1);
                }
            }
            // TODO: uncomment with advanced schedule
            //switch (endOfMonthDate.DayOfWeek)
            //{
            //    case DayOfWeek.Saturday:
            //        endOfMonthDate = endOfMonthDate.AddDays(2);
            //        break;
            //    case DayOfWeek.Sunday:
            //        endOfMonthDate = endOfMonthDate.AddDays(1);
            //        break;
            //}
            return endOfMonthDate;
        }

        public DateTime GetCurrentDate()
        {
            var calendarSet = _unitOfWork.GetDbSet<Calendar>();
            var calendar = calendarSet.SingleOrDefault();
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
            calendarSet.AddOrUpdate(calendar);
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
        }

        /// <summary>
        /// Registers monthly payment. Basically, it just transfers money to contract service loan account (3819)
        /// </summary>
        /// <param name="loan"></param>
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
            _unitOfWork.SaveChanges();
            return entry;
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
                GenerateLoanHistory(application.PersonalData, gen, history, nationalBank);
                _unitOfWork.SaveChanges();
            }
            if (application.Guarantor != null)
            {
                var guarantorId = application.Guarantor.Identification;
                var guarantorHistory = nationalBank.Where(l => l.Person.Identification == guarantorId).ToList();
                if (!guarantorHistory.Any())
                {
                    var gen = new Random();
                    GenerateLoanHistory(application.Guarantor, gen, guarantorHistory, nationalBank);
                    _unitOfWork.SaveChanges();
                }
                history = history.Concat(guarantorHistory).ToList();
            }
            return history.Distinct().OrderBy(l => l.WhenOpened);
        }

        private void GenerateLoanHistory(PersonalData personData, Random gen, List<LoanHistory> history, IDbSet<LoanHistory> nationalBank)
        {
            foreach (var histItem in 
                from i in Enumerable.Range(1, gen.Next(2, 6))
                select new DateTime(2013 - gen.Next(0, 5), gen.Next(1, 12), gen.Next(1, 25))
                into started
                let closed = started.AddMonths(gen.Next(3, 60))
                let isClosed = closed <= GetCurrentDate()
                select new LoanHistory
                {
                    Amount = gen.Next(1, 500)*10000,
                    Currency = Currency.BYR,
                    HadProblems = gen.NextDouble() > 0.85,
                    Person = personData,
                    WhenOpened = started,
                    WhenClosed = isClosed ? closed : (DateTime?) null,
                })
            {
                history.Add(histItem);
                nationalBank.AddOrUpdate(histItem);
            }
        }

        public void AddCommitteeVoting(string EmployeeId, Guid LoanApplicationId, LoanApplicationCommitteeMemberStatus Status = LoanApplicationCommitteeMemberStatus.NotConsidered)
        {
            var cvs = _unitOfWork.GetDbSet<CommitteeVoting>();
            cvs.Add(new CommitteeVoting(EmployeeId, LoanApplicationId, Status));
            _unitOfWork.SaveChanges();
        }

        public IEnumerable<CommitteeVoting> GetCommitteeVotings()
        {
            var cvs = _unitOfWork.GetDbSet<CommitteeVoting>();
            return cvs;
        }

        public ReportModel GetAnnualReport()
        {
            var today = GetCurrentDate();
            var yearAgo = today.Date.AddYears(-1);
            return GetReportForDate(today, yearAgo);
        }

        public ReportModel GetMonthlyReport()
        {
            var today = GetCurrentDate();
            var monthAgo = today.Date.AddMonths(-1);
            return GetReportForDate(today, monthAgo);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dateFrom">dateFrom is after dateTo</param>
        /// <param name="dateTo">dateTo is before dateFrom</param>
        /// <returns></returns>
        private ReportModel GetReportForDate(DateTime dateFrom, DateTime dateTo)
        {
            var report = new ReportModel(dateFrom, dateTo);
            var accountSet = _unitOfWork.GetDbSet<Account>();
            var bankAccounts = accountSet.Where(a => a.Type == AccountType.BankBalance);
            var bankAccountByr = bankAccounts.Single(a => a.Currency == Currency.BYR);
            var bankAccountEur = bankAccounts.Single(a => a.Currency == Currency.EUR);
            var bankAccountUsd = bankAccounts.Single(a => a.Currency == Currency.USD);
            report.BankIncomeByr = bankAccountByr.GetBalanceForDate(dateFrom) - bankAccountByr.GetBalanceForDate(dateTo);
            report.BankIncomeEur = bankAccountEur.GetBalanceForDate(dateFrom) - bankAccountEur.GetBalanceForDate(dateTo);
            report.BankIncomeUsd = bankAccountUsd.GetBalanceForDate(dateFrom) - bankAccountUsd.GetBalanceForDate(dateTo);

            var loans = GetLoans().Where(l => l.Application.TimeContracted >= dateTo).ToList(); // !
            report.LoansSigned = loans.Count();
            var groupedByTariffLoans = loans.GroupBy(l => l.Application.Tariff.Name).OrderBy(l => l.Count()).ToList(); // !
            report.LeastPopularTariff = groupedByTariffLoans.Any()
                ? groupedByTariffLoans.First().Key
                : "-";
            report.MostPopularTariff = groupedByTariffLoans.Any()
                ? groupedByTariffLoans.Last().Key
                : "-";
            var groupedByCurrencyLoans = loans.GroupBy(l => l.Application.Currency).ToList(); // !;
            var byrLoans = groupedByCurrencyLoans.SingleOrDefault(gr => gr.Key == Currency.BYR);
            var eurLoans = groupedByCurrencyLoans.SingleOrDefault(gr => gr.Key == Currency.EUR);
            var usdLoans = groupedByCurrencyLoans.SingleOrDefault(gr => gr.Key == Currency.USD);
            report.OverallLoanAmountIssuedByr = byrLoans != null
                ? byrLoans.Sum(l => l.Application.LoanAmount)
                : 0;
            report.OverallLoanAmountIssuedUsd = eurLoans != null
                ? eurLoans.Sum(l => l.Application.LoanAmount)
                : 0;
            report.OverallLoanAmountIssuedEur = usdLoans != null
                ? usdLoans.Sum(l => l.Application.LoanAmount)
                : 0;

            var applications = GetLoanApplications();
            var processedApplications = applications.Where(la => la.TimeCreated >= dateTo && la.Status != LoanApplicationStatus.New);
            var approvedApplications = processedApplications
                .Where(la => la.Status != LoanApplicationStatus.Rejected && la.Status != LoanApplicationStatus.New && la.Status != LoanApplicationStatus.Filled);
            report.LoanApplicationsProcessed = processedApplications.Count();
            report.LoanApplicationsApproved = approvedApplications.Count();
            return report;
        }
    }
}

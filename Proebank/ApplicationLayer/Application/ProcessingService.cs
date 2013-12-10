using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity.Core.Metadata.Edm;
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

namespace Application
{
    public class ProcessingService : IDisposable
    {
        private readonly IUnityContainer _container;
        private readonly RepositoryContainer _repositories;
        private bool _disposed;

        private static bool DaySync;
        private static bool MonthSync;
        private static readonly AccountType[] LoanAccountTypes =
        {
            AccountType.ContractService,
            AccountType.GeneralDebt,
            AccountType.Interest,
            AccountType.OverdueGeneralDebt, 
            AccountType.OverdueInterest
        };

        public ProcessingService()
        {
            _container = new UnityContainer();
            _container.LoadConfiguration();
            _repositories = new RepositoryContainer();
        }

        private IRepository<T> GetRepository<T>() where T : Entity
        {
            var repo = _repositories.Get<T>();
            if (repo != null && repo.IsDisposed)
            {
                _repositories.Remove(repo);
            }
            else if (repo == null)
            {
                repo = _container.Resolve<IRepository<T>>();
                _repositories.Add(repo);
            }
            return repo;
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
                if (date.Day == DateTime.DaysInMonth(date.Year, date.Month))
                {
                    ProcessEndOfMonth(date);
                }
                DaySync = false;
                return MoveTime(1);
            }
            return GetCurrentDate();
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
        }

        public Loan CreateLoanContract(Customer customer, LoanApplication application)
        {
            var bankAccount = GetBankAccount(application.Currency);

            var schedule = LoanCalculatePaymentSchedule(application);
            var accounts = new List<Account>(LoanAccountTypes
                .Select(accountType =>
                    CreateAccount(application.Currency, accountType)));
            var generalDebtAcc = accounts.Single(a => a.Type == AccountType.GeneralDebt);
            var entryDate = GetCurrentDate();
            var initialEntry = new Entry
            {
                Amount = application.LoanAmount,
                Currency = application.Currency,
                Date = entryDate,
                Type = EntryType.Transfer,
                SubType = EntrySubType.GeneralDebt,
            };
            application.Status = LoanApplicationStatus.Contracted;

            var bankEntry = Entry.GetOppositeFor(initialEntry);
            bankEntry.Type = EntryType.Transfer;
            bankEntry.SubType = EntrySubType.BankLoanIssued;
            AddEntry(generalDebtAcc, initialEntry);
            AddEntry(bankAccount, bankEntry);

            var loan = new Loan
            {
                Customer = customer,
                Application = application,
                IsClosed = false,
                PaymentSchedule = schedule,
                Accounts = accounts,
            };
            UpsertLoan(loan);
            return loan;
        }

        private Account GetBankAccount(Currency currency)
        {
            var accountRepo = GetRepository<Account>();
            return accountRepo.Where(acc => acc.Type == AccountType.BankBalance &&  acc.Currency == currency).Single();
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
            var entry = new Entry
            {
                Amount = amount,
                Currency = loan.Application.Currency,
                Date = GetCurrentDate(),
                Type = EntryType.Payment,
                SubType = EntrySubType.ContractService
            };
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

        private void ProcessContractServiceAccounts()
        {
            // We filter only loans with below zero balance on contract service account
            var loansWithMoneyOnServiceAccount = GetLoans(loan =>
            {
                var contractServiceAcc = loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.ContractService);
                return contractServiceAcc != null && contractServiceAcc.Balance > 0;
            });
            foreach (var loan in loansWithMoneyOnServiceAccount)
            {
                var accounts = loan.Accounts;
                var contractAccount = loan.Accounts.Single(a => a.Type == AccountType.ContractService);
                var amount = contractAccount.Balance;

                if (amount > 0M)
                {
                    // at first we transfer money to interest account
                    // then to generalDebtAccount
                    var interestAccount = accounts.Single(acc => acc.Type == AccountType.Interest);
                    var interestPayment = Math.Min(amount, interestAccount.Balance);
                    if (interestPayment > 0M)
                    {
                        var interestEntryPlus = new Entry
                        {
                            Amount = interestPayment,
                            Currency = loan.Application.Currency,
                            Date = GetCurrentDate(),
                            Type = EntryType.Payment,
                            SubType = EntrySubType.Interest
                        };
                        var interestEntryMinus = Entry.GetOppositeFor(interestEntryPlus);
                        AddEntry(interestAccount, interestEntryPlus);
                        AddEntry(contractAccount, interestEntryMinus);
                        amount -= interestPayment;
                    }
                    var generalDebtAccount = accounts.Single(acc => acc.Type == AccountType.GeneralDebt);
                    var generalDebtPayment = Math.Min(amount, generalDebtAccount.Balance);
                    if (generalDebtPayment > 0M)
                    {
                        var generalDebtPlus = new Entry
                        {
                            Amount = generalDebtPayment,
                            Currency = loan.Application.Currency,
                            Date = GetCurrentDate(),
                            Type = EntryType.Payment,
                            SubType = EntrySubType.GeneralDebt
                        };
                        var generalDebtMinus = Entry.GetOppositeFor(generalDebtPlus);
                        AddEntry(generalDebtAccount, generalDebtPlus);
                        AddEntry(contractAccount, generalDebtMinus);
                    }
                }
            }
            UpdateDailyProcessingTime();
        }

        private PaymentSchedule LoanCalculatePaymentSchedule(LoanApplication loanApplication)
        {
            return PaymentScheduleCalculator.Calculate(loanApplication);
        }

        private Dictionary<Account, Entry> LoanProcessEndOfMonth(DateTime currentDate)
        {
            var loanRepository = GetRepository<Loan>();
            return loanRepository.GetAll()
                .Where(l => !l.IsClosed)
                .ToDictionary(
                    loan => loan.Accounts.Single(acc => acc.Type == AccountType.Interest),
                    loan => InterestCalculator.CalculateInterestFor(loan, currentDate));
        }

        #region Loan service methods
        public IEnumerable<Loan> GetLoans(Func<Loan, bool> filter)
        {
            var loanRepository = GetRepository<Loan>();
            return loanRepository.Where(filter);
        }

        private void UpsertLoan(Loan loan)
        {
            var loanRepo = GetRepository<Loan>();
            loanRepo.AddOrUpdate(loan);
            loanRepo.SaveChanges();
        }

        private bool CanLoanBeClosed(Loan loan)
        {
            return loan.Accounts.All(a => a.Balance == 0M);
        }

        private void CloseLoan(Loan loan)
        {
            var loanRepo = GetRepository<Loan>();
            loan.IsClosed = true;
            loanRepo.AddOrUpdate(loan);
            loanRepo.SaveChanges();
        }
	    #endregion

        #region Loan application service methods
        public IEnumerable<LoanApplication> GetLoanApplications(bool showRemoved = false)
        {
            var loanApplicationRepo = GetRepository<LoanApplication>();
            return loanApplicationRepo.GetAll(showRemoved);
        }

        public IEnumerable<LoanApplication> GetLoanApplications(Func<LoanApplication, bool> filter)
        {
            var loanApplicationRepo = GetRepository<LoanApplication>();
            return loanApplicationRepo.Where(filter);
        }

        public void UpsertLoanApplication(LoanApplication loanApplication)
        {
            var loanApplicationRepo = GetRepository<LoanApplication>();
            loanApplicationRepo.AddOrUpdate(loanApplication);
            loanApplicationRepo.SaveChanges();
        }

        public void DeleteLoanApplicationById(Guid id)
        {
            var loanApplicationRepo = GetRepository<LoanApplication>();
            var loanApplication = loanApplicationRepo.Where(la => la.Id.Equals(id)).Single();
            loanApplicationRepo.Remove(loanApplication);
            loanApplicationRepo.SaveChanges();
        }

        public void CreateLoanApplication(LoanApplication loanApplication, bool fromConsultant = false)
        {
            loanApplication.Documents = new List<Document>();
            loanApplication.TimeCreated = GetCurrentDate();
            loanApplication.Status = fromConsultant ? LoanApplicationStatus.InitiallyApproved : LoanApplicationStatus.New;
            var selectedTariff = GetTariffs(t => t.Id.Equals(loanApplication.TariffId)).Single();
            //loanApplication.Tariff = selectedTariff;
            loanApplication.LoanPurpose = selectedTariff.LoanPurpose;
            loanApplication.Currency = selectedTariff.Currency;

            var loanApplicationRepo = GetRepository<LoanApplication>();
            var validationResult = ValidateLoanApplication(loanApplication);
            if (validationResult.Count == 0)
            {
                loanApplicationRepo.AddOrUpdate(loanApplication);
                loanApplicationRepo.SaveChanges();
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
            var loanRepo = GetRepository<LoanApplication>();
            loanApplication.Status = LoanApplicationStatus.Approved;
            loanRepo.AddOrUpdate(loanApplication);
            loanRepo.SaveChanges();
        }

        public void RejectLoanApplication(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.Rejected;
            var loanRepository = GetRepository<LoanApplication>();
            loanRepository.AddOrUpdate(loanApplication);
            loanRepository.SaveChanges();
        }

        public void SendLoanApplicationToCommittee(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.UnderCommitteeConsideration;
            var loanRepository = GetRepository<LoanApplication>();
            loanRepository.AddOrUpdate(loanApplication);
            loanRepository.SaveChanges();
        }

        public void SendLoanApplicationToSecurity(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.UnderRiskConsideration;
            var loanRepository = GetRepository<LoanApplication>();
            loanRepository.AddOrUpdate(loanApplication);
            loanRepository.SaveChanges();
        }
        #endregion

        #region Tariff service methods
        public IEnumerable<Tariff> GetTariffs()
        {
            var tariffRepo = GetRepository<Tariff>();
            return tariffRepo.GetAll();
        }

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, bool> filter)
        {
            var tariffRepo = GetRepository<Tariff>();
            return tariffRepo.Where(filter);
        }

        public void UpsertTariff(Tariff tariff)
        {
            var tariffRepo = GetRepository<Tariff>();
            tariffRepo.AddOrUpdate(tariff);
            tariffRepo.SaveChanges();
        }

        public void DeleteTariffById(Guid id)
        {
            var tariffRepo = GetRepository<Tariff>();
            var tariff = tariffRepo.GetAll().Single(t => t.Id.Equals(id));
            tariff.EndDate = GetCurrentDate();
            tariffRepo.Remove(tariff);
            tariffRepo.SaveChanges();
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
            var tariff = GetRepository<Tariff>().GetAll().Single(t => t.Id == loanApplication.TariffId);
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
            GetRepository<Calendar>().SaveChanges();
            return result;
        }

        private Calendar GetCalendar(bool withLock = false)
        {
            var calendarRepository = GetRepository<Calendar>();
            var calendar = calendarRepository.GetAll().Single();
            if (withLock)
            {
                if (!calendar.ProcessingLock)
                {
                    calendar.ProcessingLock = true;
                    calendarRepository.AddOrUpdate(calendar);
                    calendarRepository.SaveChanges();
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
            var calendarRepository = GetRepository<Calendar>();
            var currentCalendar = calendarRepository.GetAll().First();
            currentCalendar.LastDailyProcessingTime = currentCalendar.CurrentTime;
            calendarRepository.AddOrUpdate(currentCalendar);
            calendarRepository.SaveChanges();
        }

        private void UpdateMonthlyProcessingTime()
        {
            var calendarRepo = GetRepository<Calendar>();
            var currentCalendar = calendarRepo.GetAll().First();
            currentCalendar.LastMonthlyProcessingTime = currentCalendar.CurrentTime;
            calendarRepo.AddOrUpdate(currentCalendar);
            calendarRepo.SaveChanges();
        }

        public DateTime GetCurrentDate()
        {
            var calendarRepo = GetRepository<Calendar>();
            var calendar = calendarRepo.GetAll().SingleOrDefault();
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
            var calendarRepo = GetRepository<Calendar>();
            if (calendarRepo.GetAll().Any())
            {
                calendarRepo.GetAll().First().CurrentTime = dateTime;
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
            calendarRepo.SaveChanges();
        }
        #endregion

        #region Account service
        public Account CreateAccount(Currency currency, AccountType accountType)
        {
            var accountRepo = GetRepository<Account>();
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
            accountRepo.SaveChanges();
            return acc;
        }

        public void AddEntry(Account account, Entry entry)
        {
            var accountRepo = GetRepository<Account>();
            if (account == null)
                throw new ArgumentNullException("account");
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (account.Currency != entry.Currency)
                throw new ArgumentException("Currencies are not equal");
            account.Entries.Add(entry);
            accountRepo.AddOrUpdate(account);
            accountRepo.SaveChanges();
        }

        public void CloseAccount(Account account)
        {
            var accountRepo = GetRepository<Account>();
            if (account.IsClosed)
            {
                throw new ArgumentException("Account is already closed");
            }
            account.IsClosed = true;
            account.DateClosed = GetCurrentDate();
            accountRepo.AddOrUpdate(account);
        }
        #endregion

        public List<LoanHistory> GetHistoryFromNationalBank(LoanApplication application)
        {
            var nationalBank = GetRepository<LoanHistory>();
            var doc =
                application.Documents.Single(
                    d => d.DocType == DocType.Passport && d.TariffDocType == TariffDocType.DebtorPrimary);
            var history = nationalBank.GetAll().Where(l => l.Person.Id == doc.Id).ToList();
            if (!history.Any())
            {
                var gen = new Random();
                if (gen.NextDouble() > 0.4)
                {
                    var started = new DateTime(2013 - gen.Next(0, 5), gen.Next(1, 12), gen.Next(1, 25));
                    var closed = started.AddMonths(gen.Next(3, 60));
                    var isClosed = closed <= GetCurrentDate();
                    foreach (var i in Enumerable.Range(1, gen.Next(2, 6)))
                    {
                        var histItem = new LoanHistory
                        {
                            Amount = gen.Next(1, 500)*10000,
                            HadProblems = gen.NextDouble() > 0.85,
                            Person = doc,
                            WhenOpened = started,
                            WhenClosed = isClosed ? closed : (DateTime?) null,
                        };
                        history.Add(histItem);
                    }
                    nationalBank.SaveChanges();
                }
            }
            return history;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    var allRepos = _repositories.GetAll();
                    foreach (var repository in allRepos.OfType<IDisposable>())
                    {
                        repository.Dispose();
                    }
                    allRepos.Clear();
                    _container.Dispose();
                }
                _disposed = true;
            }
        }
    }
}

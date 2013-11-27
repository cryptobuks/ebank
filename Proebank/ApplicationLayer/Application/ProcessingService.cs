﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Application.LoanProcessing;
using Domain;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Repositories;
using Infrastructure;
using Infrastructure.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Application
{
    // TODO: make all services disposable
    public class ProcessingService
    {
        public readonly LoanRepository LoanService;
        private readonly AbstractDataContext Context;
        private readonly AccountRepository AccountService;
        private readonly CalendarService CalendarService;
        private static readonly object DaySync = new object();
        private static readonly object MonthSync = new object();

        public ProcessingService(LoanRepository loanService, AccountRepository accountService, CalendarService calendarService, 
            AbstractDataContext context)
        {
            LoanService = loanService;
            AccountService = accountService;
            CalendarService = calendarService;
            Context = context;
        }

        /// <summary>
        /// Process to end every banking day
        /// </summary>
        /// <param name="date">current day</param>
        public DateTime ProcessEndOfDay()
        {
            // TODO: lock any other account operations!
            lock (DaySync)
            {
                var date = CalendarService.Calendar.CurrentTime.HasValue 
                    ? CalendarService.Calendar.CurrentTime.Value
                    : DateTime.UtcNow;
                ProcessContractServiceAccounts();
                if (date.Day == DateTime.DaysInMonth(date.Year, date.Month))
                {
                    ProcessEndOfMonth(date);
                }
                return CalendarService.MoveTime(1);
            }
        }

        public void ProcessEndOfMonth(DateTime date)
        {
            lock (MonthSync)
            {
                var accruals = LoanProcessEndOfMonth(date);
                foreach (var accrual in accruals)
                {
                    AccountService.AddEntry(accrual.Key, accrual.Value);
                }
                CalendarService.UpdateMonthlyProcessingTime();
            }
        }

        public Loan CreateLoanContract(LoanApplication application)
        {
            // TODO: CRITICAL: check bank balance
            var schedule = LoanCalculatePaymentSchedule(application);
            var accounts = new List<Account>(LoanRepository.AccountTypes
                .Select(accountType =>
                    AccountService.CreateAccount(application.Currency, accountType)));
            var generalDebtAcc = accounts.Single(a => a.Type == AccountType.GeneralDebt);
            var entryDate = DateTime.UtcNow;
            var initialEntry = new Entry()
            {
                Amount = application.LoanAmount,
                Currency = application.Currency,
                Date = entryDate,
                Type = EntryType.Transfer,
                SubType = EntrySubType.GeneralDebt,
            };
            application.Status = LoanApplicationStatus.Contracted;
            // TODO: CRITICAL: add entry to bank balance
            AccountService.AddEntry(generalDebtAcc, initialEntry); 

            var userStore = new UserStore<Customer>(Context);
            var userManager = new UserManager<Customer>(userStore);
            var user = new Customer
            {
                UserName = "Username" + DateTime.Now.ToFileTime(),
                Address = "No.Address",
                BirthDate = DateTime.Today,
                Email = "no@ema.il",
                FirstName = "Firstname",
                LastName = "Lastname",
                MiddleName = "Middlename",
                IdentificationNumber = "0000-0000-0000"
            };
            var identityResult = userManager.Create(user, "11111111");
            if (!identityResult.Succeeded)
            {
                return null;
            }
            var loan = new Loan
            {
                Customer = user,
                Application = application,
                IsClosed = false,
                PaymentSchedule = schedule,
                Accounts = accounts,
            };
            LoanService.UpsertLoan(loan);
            return loan;
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
            var entry = new Entry()
            {
                Amount = amount,
                Currency = loan.Application.Currency,
                Date = DateTime.UtcNow,
                Type = EntryType.Payment,
                SubType = EntrySubType.ContractService
            };
            AccountService.AddEntry(contractAccount, entry);
            return entry;
        }

        public bool CloseLoanContract(Loan loan)
        {
            var canBeClosed = LoanService.CanLoanBeClosed(loan);
            if (canBeClosed)
            {
                foreach (var account in loan.Accounts)
                {
                    AccountService.CloseAccount(account);
                }
                LoanService.CloseLoan(loan);
            }
            return canBeClosed;
        }

        public Calendar GetCurrentDateTime()
        {
            return CalendarService.Calendar;
        }

        public void SetCurrentDateTime(DateTime dateTime)
        {
            // TODO: as it is public some checks should be done
            CalendarService.SetCurrentDate(dateTime);
        }

        private void ProcessContractServiceAccounts()
        {
            // TODO: transfer from 3819 to other accounts, not only two
            // We filter only loans with below zero balance on contract service account
            var loansWithMoneyOnServiceAccount = LoanService.GetLoans(loan =>
            {
                var contractServiceAcc = loan.Accounts.FirstOrDefault(acc => acc.Type == AccountType.ContractService);
                return contractServiceAcc != null && contractServiceAcc.Balance > 0;
            });
            foreach (var loan in loansWithMoneyOnServiceAccount)
            {
                var accounts = loan.Accounts;
                // TODO: make filters static
                var contractAccount = loan.Accounts.Single(acc => acc.Type == AccountType.ContractService);
                var amount = contractAccount.Balance;

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
                        Date = DateTime.UtcNow,
                        Type = EntryType.Payment,
                        SubType = EntrySubType.Interest
                    };
                    var interestEntryMinus = Entry.GetOppositeFor(interestEntryPlus);
                    AccountService.AddEntry(interestAccount, interestEntryPlus);
                    AccountService.AddEntry(contractAccount, interestEntryMinus);
                    amount -= interestPayment;
                    if (amount > 0M)
                    {
                        var generalDebtAccount = accounts.Single(acc => acc.Type == AccountType.GeneralDebt);
                        var generalDebtPayment = Math.Min(amount, generalDebtAccount.Balance);
                        if (generalDebtPayment > 0M)
                        {
                            var generalDebtPlus = new Entry
        {
                                Amount = generalDebtPayment,
                                Currency = loan.Application.Currency,
                                Date = DateTime.UtcNow,
                                Type = EntryType.Payment,
                                SubType = EntrySubType.GeneralDebt
                            };
                            var generalDebtMinus = Entry.GetOppositeFor(generalDebtPlus);
                            AccountService.AddEntry(generalDebtAccount, generalDebtPlus);
                            AccountService.AddEntry(contractAccount, generalDebtMinus);
                        }
                    }
                }
            }
            CalendarService.UpdateDailyProcessingTime();
        }

        public PaymentSchedule LoanCalculatePaymentSchedule(LoanApplication loanApplication)
        {
            return PaymentScheduleCalculator.Calculate(loanApplication);
        }

        public Dictionary<Account, Entry> LoanProcessEndOfMonth(DateTime currentDate)
        {
            return Context.Loans
                .Where(l => !l.IsClosed)
                .ToDictionary(
                    loan => loan.Accounts.Single(acc => acc.Type == AccountType.Interest),
                    loan => InterestCalculator.CalculateInterestFor(loan, currentDate));
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Application.AccountProcessing;
using Application.LoanProcessing;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;
using Infrastructure.Migrations;

namespace Application
{
    public class ProcessingService
    {
        private readonly LoanService _loanService;
        private readonly AccountService _accountService;
        private static readonly object DaySync = new object();
        private static readonly object MonthSync = new object();

        public ProcessingService(LoanService loanService, AccountService accountService)
        {
            _loanService = loanService;
            _accountService = accountService;
        }

        /// <summary>
        /// Process to end every banking day
        /// </summary>
        /// <param name="date">current day</param>
        public void ProcessEndOfDay(DateTime date)
        {
            // TODO: lock any other account operations!
            lock (DaySync)
            {
                ProcessContractServiceAccounts();
                if (date.Day == DateTime.DaysInMonth(date.Year, date.Month))
                {
                    ProcessEndOfMonth(date);
                }
            }
        }

        private void ProcessContractServiceAccounts()
        {
            // TODO: transfer from 3819 to other accounts, not only two
            // We filter only loans with below zero balance on contract service account
            var loansWithMoneyOnServiceAccount = _loanService.GetLoans(loan =>
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
                    _accountService.AddEntry(interestAccount, interestEntryPlus);
                    _accountService.AddEntry(contractAccount, interestEntryMinus);
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
                            _accountService.AddEntry(generalDebtAccount, generalDebtPlus);
                            _accountService.AddEntry(contractAccount, generalDebtMinus);
                        }
                    }
                }
            }
        }

        public void ProcessEndOfMonth(DateTime date)
        {
            lock (MonthSync)
            {
                var accruals = _loanService.ProcessEndOfMonth(date);
                foreach (var accrual in accruals)
                {
                    _accountService.AddEntry(accrual.Key, accrual.Value);
                }
            }
        }

        public Loan CreateLoanContract(LoanApplication application)
        {
            // TODO: CRITICAL: check bank balance
            application.Contract();
            var schedule = _loanService.CalculatePaymentSchedule(application);
            var accounts = new List<Account>(LoanService.AccountTypes
                .Select(accountType =>
                    _accountService.CreateAccount(application.Currency, accountType)));
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
            // TODO: CRITICAL: add entry to bank balance
            _accountService.AddEntry(generalDebtAcc, initialEntry); 
            var loan = new Loan
            {
                Application = application,
                IsClosed = false,
                PaymentSchedule = schedule,
                Accounts = accounts,
            };
            _loanService.SaveNewLoan(loan);
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
            _accountService.AddEntry(contractAccount, entry);
            return entry;
        }

        public void CloseLoanContract()
        {
            throw new System.NotImplementedException();
        }
    }
}

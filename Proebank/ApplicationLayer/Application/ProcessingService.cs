using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Application.AccountProcessing;
using Application.LoanProcessing;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

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

        public void ProcessEndOfDay(DateTime date)
        {
            lock (DaySync)
            {
                throw new NotImplementedException();
            }
        }

        public Loan CreateLoanContract(LoanApplication application)
        {
            // TODO: check bank balance
            application.Contract();
            var schedule = _loanService.CalculatePaymentSchedule(application);
            var accounts = new List<Account>(LoanService.AccountTypes
                .Select(accountType =>
                    _accountService.CreateAccount(application.Currency, accountType)));
            var generalDebtAcc = accounts.First(a => a.Type == AccountType.GeneralDebt);
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

        public void CloseLoanContract()
        {
            throw new System.NotImplementedException();
        }
    }
}

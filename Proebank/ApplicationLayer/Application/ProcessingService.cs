using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Application.AccountProcessing;
using Application.LoanProcessing;

namespace Application
{
    public static class ProcessingService
    {
        private static readonly object DaySync = new object();
        private static readonly object MonthSync = new object();

        public static void ProcessEndOfMonth(DateTime date, LoanService loanService, AccountService accountService)
        {
            lock (MonthSync)
            {
                var accruals = loanService.ProcessEndOfMonth(date);
                foreach (var accrual in accruals)
                {
                    accountService.AddEntry(accrual.Key, accrual.Value);
                }
            }
        }

        public static void ProcessEndOfDay(DateTime date, LoanService loanService, AccountService accountService)
        {
            lock (DaySync)
            {
                throw new NotImplementedException();
            }
        }
    }
}

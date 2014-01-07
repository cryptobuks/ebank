using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    static class InterestCalculator
    {
        public static void CalculateInterestFor(Loan loan, DateTime date, Entry destinyEntry)
        {
            UseBasicLogic(loan, date, destinyEntry);
        }

        private static void UseBasicLogic(Loan loan, DateTime date, Entry destinyEntry)
        {
            // very basic logic
            var payments =
                loan.PaymentSchedule.Payments
                    .Where(p => p.AccruedOn.HasValue && p.AccruedOn.Value.Date == date.Date)
                    .ToList();

            destinyEntry.Amount = payments.Sum(p => p.AccruedInterestAmount);
            if (destinyEntry.Amount == 0) throw new Exception("zero-equal transfer");
            destinyEntry.Currency = loan.Application.Currency;
            destinyEntry.Type = EntryType.Accrual;
            destinyEntry.SubType = EntrySubType.Interest;
            destinyEntry.Date = date;
        }

        private static void UseAdvancedLogic(Loan loan, DateTime date, Entry destinyEntry)
        {
            var payments = loan.PaymentSchedule.Payments.ToList();
            var currentPmtIndex = payments.FindIndex(p => p.AccruedOn.HasValue && p.AccruedOn.Value.Date == date.Date);
            var accruedOn = payments[currentPmtIndex - 1].AccruedOn;
            DateTime startDate;
            if (accruedOn != null)
            {
                startDate = currentPmtIndex == 0 && loan.Application.TimeContracted.HasValue
                    ? loan.Application.TimeContracted.Value.Date
                    : (accruedOn.Value);
            }
            else
            {
                startDate = date.AddMonths(-1);
            }
            var endDate = date;
            var dt = startDate;

            var mainDebtAccount = loan.Accounts.Single(a => a.Type == AccountType.GeneralDebt);
            var startBalance = mainDebtAccount.GetBalanceForDate(startDate);
            var endBalance = mainDebtAccount.GetBalanceForDate(endDate);
            if (startBalance == endBalance)
            {
                // use basic logic
            }
            else
            {
                while (dt.Date != endDate)
                {
                    // check balance for every day
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    public static class InterestCalculator
    {
        public static void CalculateInterestFor(Loan loan, DateTime date, Entry destinyEntry)
        {
            //UseBasicLogic(loan, date, destinyEntry);
            UseAdvancedLogic(loan, date, destinyEntry);
            destinyEntry.Currency = loan.Application.Currency;
            destinyEntry.Type = EntryType.Accrual;
            destinyEntry.SubType = EntrySubType.Interest;
            destinyEntry.Date = date;
        }

        public static decimal CalculateInterestForCustomerInformation(Loan loan, DateTime date)
        {
            try
            {
                var mainDebtAccount = loan.Accounts.Single(a => a.Type == AccountType.GeneralDebt);
                var result =
                    Math.Round(
                        mainDebtAccount.GetBalanceForDate(date)*loan.Application.Tariff.InterestRate/360 + 0.005M, 2);
                return result;
            }
            catch (Exception)
            {
                return -1M;
            }
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
        }

        private static void UseAdvancedLogic(Loan loan, DateTime date, Entry destinyEntry)
        {
            var payments = loan.PaymentSchedule.Payments.OrderBy(p => p.AccruedOn).ToList();
            var currentPmtIndex = payments.FindIndex(p => p.AccruedOn.HasValue && p.AccruedOn.Value.Date == date.Date);
            DateTime startDate;
            if (currentPmtIndex == 0 && loan.Application.TimeContracted.HasValue)
            {
                startDate = loan.Application.TimeContracted.Value.Date;
            }
            else
            {
                var accruedOn = payments[currentPmtIndex - 1].AccruedOn;
                startDate = accruedOn.HasValue ? accruedOn.Value : date.AddMonths(-1);
            }
            var endDate = date;

            var mainDebtAccount = loan.Accounts.Single(a => a.Type == AccountType.GeneralDebt);
            var mainDebtMonthlySum = 0M;
            for (var dt = startDate.AddDays(1).Date; dt <= endDate.Date; dt = dt.AddDays(1)) // TODO: check for dt < endDate.Date (or any bound values)
            {
                mainDebtMonthlySum += mainDebtAccount.GetBalanceForDate(dt) * loan.Application.Tariff.InterestRate / 360;
            }
            destinyEntry.Amount = Math.Round(mainDebtMonthlySum, 2);
        }
    }
}

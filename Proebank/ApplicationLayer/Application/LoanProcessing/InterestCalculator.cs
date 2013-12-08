using System;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    static class InterestCalculator
    {
        public static Entry CalculateInterestFor(Loan loan, DateTime date)
        {
            var payments = loan.PaymentSchedule.Payments.Where(p => p.ShouldBePaidBefore.Month == date.Month);
            return new Entry
            {
                Amount = payments.Sum(p => p.Amount),
                Currency = loan.Application.Currency,
                Type = EntryType.Accrual,
                SubType = EntrySubType.Interest,
                Date = date
            };
        }

        public static decimal TotalSum(Tariff tariff, decimal sum, int term)
        {
            return sum + sum*(term*tariff.InterestRate/12);
        }
    }
}

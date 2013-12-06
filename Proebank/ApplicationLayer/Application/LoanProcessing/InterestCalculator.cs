using System;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    static class InterestCalculator
    {
        public static Entry CalculateInterestFor(Loan loan, DateTime date)
        {
            // TODO: very basic logic. Improve later
            var application = loan.Application;
            var amount = application.LoanAmount;
            var interestRate = application.Tariff.InterestRate;
            // TODO: take from schedule for current date
            var accrual = amount * interestRate / application.Term;
            return new Entry {Amount = accrual, Type = EntryType.Accrual, SubType = EntrySubType.Interest};
        }
    }
}

using System;
using Application.FinancialFunctions;
using Domain.Models.Accounts;

namespace Application.LoanProcessing
{
    static class FeeCalculator
    {
        // TODO: complete it and use!
        public static void Calculate()
        {
            var a = Interest.InterestRate(4, 5);
        }

        public static Entry AccrualFor(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}

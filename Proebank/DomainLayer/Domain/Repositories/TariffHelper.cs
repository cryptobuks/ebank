using System;
using Domain.Models.Loans;

namespace Domain.Repositories
{
    class TariffHelper
    {
        public bool ValidateLoanApplication(LoanApplication loanApplication)
        {
            if (loanApplication == null || loanApplication.Tariff == null)
            {
                throw new ArgumentException("loanApplication");
            }
            return loanApplication.Tariff.Validate(loanApplication);
        }
    }
}

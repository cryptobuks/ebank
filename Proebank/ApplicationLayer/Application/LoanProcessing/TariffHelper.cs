using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    class TariffHelper
    {
        private readonly ITariffRepository _repository;

        public TariffHelper(ITariffRepository repository)
        {
            _repository = repository;
        }

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

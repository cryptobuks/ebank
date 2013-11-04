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
        private ITariffRepository _repository;
        public TariffHelper(ITariffRepository repository)
        {
            _repository = repository;
        }

        public bool ValidateLoanApplication(LoanApplication loanApplication)
        {
            var tariff = _repository.Get(t => t.Id == loanApplication.Id);
            if (tariff == null)
            {
                throw new Exception("Tariff with id=" + loanApplication.Id + " not found");
            }
            return tariff.Validate(loanApplication);
        }
    }
}

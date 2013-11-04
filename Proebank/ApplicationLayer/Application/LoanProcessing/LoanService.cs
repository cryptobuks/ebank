using System.Collections.Generic;
using Domain.Models;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    public class LoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ITariffRepository _tariffRepository;
        private readonly TariffHelper _tariffHelper;

        public LoanService(ILoanRepository loanRepository, 
            ILoanApplicationRepository loanApplicationRepository,
            ITariffRepository tariffRepository)
        {
            _loanRepository = loanRepository;
            _loanApplicationRepository = loanApplicationRepository;
            _tariffRepository = tariffRepository;
            _tariffHelper = new TariffHelper(_tariffRepository);
        }

        public bool CreateLoanApplication(LoanApplication loanApplication)
        {
            var validationResult = _tariffHelper.ValidateLoanApplication(loanApplication);
            if (validationResult)
            {
                _loanApplicationRepository.SaveOrUpdate(loanApplication);
            }
            return validationResult;
        }

        public void ConsiderLoanApplication(LoanApplication loanApplication, bool decision)
        {
            // TODO: change later
            if (decision)
            {
                loanApplication.Approve();
            }
            else
            {
                loanApplication.Reject();
            }
        }

        private bool ValidateLoanApplication(LoanApplication loanApplication)
        {
            throw new System.NotImplementedException();
        }

        public void CreateLoanContract()
        {
            throw new System.NotImplementedException();
        }

        public void CloseLoanContract()
        {
            throw new System.NotImplementedException();
        }

        public void RegisterPayment()
        {
            throw new System.NotImplementedException();
        }
    }
}

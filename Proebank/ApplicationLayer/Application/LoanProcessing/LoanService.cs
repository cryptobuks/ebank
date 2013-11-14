using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;
using Infrastructure;

namespace Application.LoanProcessing
{
    public class LoanService
    {
        //private readonly ILoanRepository _unitOfWork.LoanRepository;
        //private readonly ILoanApplicationRepository _unitOfWork.LoanApplicationRepository;
        //private readonly ITariffRepository _tariffRepository;
        private IUnitOfWork _unitOfWork;
        private readonly TariffHelper _tariffHelper;
        private static readonly AccountType[] LoanAccountTypes = new[]
                {
                    AccountType.ContractService,
                    AccountType.GeneralDebt,
                    AccountType.Interest,
                    AccountType.OverdueGeneralDebt, 
                    AccountType.OverdueInterest,
                };

        public LoanService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _tariffHelper = new TariffHelper();
        }

        public static AccountType[] AccountTypes
        {
            get
            {
                return LoanAccountTypes;
            }
        }

        public bool CreateLoanApplication(LoanApplication loanApplication)
        {
            var validationResult = _tariffHelper.ValidateLoanApplication(loanApplication);
            if (validationResult)
            {
                _unitOfWork.LoanApplicationRepository.SaveOrUpdate(loanApplication);
            }
            return validationResult;
        }

        public void ConsiderLoanApplication(LoanApplication loanApplication, bool decision)
        {
            // TODO: change later
            if (decision)
            {
                _unitOfWork.LoanApplicationRepository.Approve(loanApplication);
            }
            else
            {
                _unitOfWork.LoanApplicationRepository.Reject(loanApplication);
            }
        }

        public PaymentSchedule CalculatePaymentSchedule(LoanApplication loanApplication)
        {
            return PaymentScheduleCalculator.Calculate(loanApplication);
        }

        public Dictionary<Account, Entry> ProcessEndOfMonth(DateTime currentDate)
        {
            return _unitOfWork.LoanRepository
                .GetAll(l => !l.IsClosed)
                .ToDictionary(
                    loan => loan.Accounts.Single(acc => acc.Type == AccountType.Interest),
                    loan => InterestCalculator.CalculateInterestFor(loan, currentDate));
        }

        public void SaveOrUpdateTariff(Tariff tariff)
        {
            _unitOfWork.TariffRepository.SaveOrUpdate(tariff);
        }

        internal void SaveNewLoan(Loan loan)
        {
            // TODO: check if application is saved without call to application repository
            //_unitOfWork.LoanApplicationRepository.SaveOrUpdate(loan.Application);
            _unitOfWork.LoanRepository.SaveOrUpdate(loan);
        }

        internal bool CanLoanBeClosed(Loan loan)
        {
            return loan.Accounts.All(a => a.Balance == 0M);
        }

        internal void CloseLoan(Loan loan)
        {
            loan.IsClosed = true;
            _unitOfWork.LoanRepository.SaveOrUpdate(loan);
        }

        // TODO: do we need such methods?
        public IEnumerable<Loan> GetAll()
        {
            return _unitOfWork.LoanRepository.GetAll();
        }

        public Loan GetSingle(Func<Loan, bool> filter)
        {
            return _unitOfWork.LoanRepository.Get(filter);
        }

        public IEnumerable<Loan> GetLoans(Func<Loan, bool> filter)
        {
            return _unitOfWork.LoanRepository.GetAll(filter);
        }

        public LoanApplication GetApplication(Guid loanApplicationId)
        {
            return _unitOfWork.LoanApplicationRepository.Get(la => la.Id == loanApplicationId);
        }
    }
}

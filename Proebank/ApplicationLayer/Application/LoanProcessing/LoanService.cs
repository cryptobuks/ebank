using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    public class LoanService
    {
        private readonly ILoanRepository _loanRepository;
        private readonly ILoanApplicationRepository _loanApplicationRepository;
        private readonly ITariffRepository _tariffRepository;
        private readonly TariffHelper _tariffHelper;
        private static readonly AccountType[] LoanAccountTypes = new[]
                {
                    AccountType.ContractService,
                    AccountType.GeneralDebt,
                    AccountType.Interest,
                    AccountType.OverdueGeneralDebt, 
                    AccountType.OverdueInterest,
                };

        public LoanService(ILoanRepository loanRepository, 
            ILoanApplicationRepository loanApplicationRepository,
            ITariffRepository tariffRepository)
        {
            _loanRepository = loanRepository;
            _loanApplicationRepository = loanApplicationRepository;
            _tariffRepository = tariffRepository;
            _tariffHelper = new TariffHelper(_tariffRepository);
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
                _loanApplicationRepository.SaveOrUpdate(loanApplication);
            }
            return validationResult;
        }

        public void ConsiderLoanApplication(LoanApplication loanApplication, bool decision)
        {
            // TODO: change later
            if (decision)
            {
                _loanApplicationRepository.Approve(loanApplication);
            }
            else
            {
                _loanApplicationRepository.Reject(loanApplication);
            }
        }

        public PaymentSchedule CalculatePaymentSchedule(LoanApplication loanApplication)
        {
            return PaymentScheduleCalculator.Calculate(loanApplication);
        }

        public Dictionary<Account, Entry> ProcessEndOfMonth(DateTime currentDate)
        {
            return _loanRepository
                .GetAll(l => !l.IsClosed)
                .ToDictionary(
                    loan => loan.Accounts.Single(acc => acc.Type == AccountType.Interest),
                    loan => InterestCalculator.CalculateInterestFor(loan, currentDate));
        }

        public void SaveOrUpdateTariff(Tariff tariff)
        {
            _tariffRepository.SaveOrUpdate(tariff);
        }

        internal void SaveNewLoan(Loan loan)
        {
            // TODO: check if application is saved without call to application repository
            _loanApplicationRepository.SaveOrUpdate(loan.Application);
            _loanRepository.SaveOrUpdate(loan);
        }

        internal bool CanLoanBeClosed(Loan loan)
        {
            return loan.Accounts.All(a => a.Balance == 0M);
        }

        internal void CloseLoan(Loan loan)
        {
            loan.IsClosed = true;
            _loanRepository.SaveOrUpdate(loan);
        }

        // TODO: do we need such methods?
        public IEnumerable<Loan> GetAll()
        {
            return _loanRepository.GetAll();
        }

        public Loan GetSingle(Func<Loan, bool> filter)
        {
            return _loanRepository.Get(filter);
        }

        public IEnumerable<Loan> GetLoans(Func<Loan, bool> filter)
        {
            return _loanRepository.GetAll(filter);
        }
    }
}

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

        public void CreateLoanApplication(LoanApplication loanApplication)
        {
            var validationResult = _tariffHelper.ValidateLoanApplication(loanApplication);
            if (validationResult)
            {
                _unitOfWork.LoanApplicationRepository.Upsert(loanApplication);
                _unitOfWork.Save();
            }
            // TODO: make it without exception :)
            else throw new Exception("Loan application is not valid");
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

        public void UpsertTariff(Tariff tariff)
        {
            _unitOfWork.TariffRepository.Upsert(tariff);
            _unitOfWork.Save();
        }

        internal void UpsertLoan(Loan loan)
        {
            _unitOfWork.LoanRepository.Upsert(loan);
            _unitOfWork.Save();
        }

        public void UpsertLoanApplication(LoanApplication loanApplication)
        {
            _unitOfWork.LoanApplicationRepository.Upsert(loanApplication);
            _unitOfWork.Save();
        }

        internal bool CanLoanBeClosed(Loan loan)
        {
            return loan.Accounts.All(a => a.Balance == 0M);
        }

        internal void CloseLoan(Loan loan)
        {
            loan.IsClosed = true;
            _unitOfWork.LoanRepository.Upsert(loan);
            _unitOfWork.Save();
        }

        public IEnumerable<Loan> GetLoans(Func<Loan, bool> filter)
        {
            return _unitOfWork.LoanRepository.GetAll(filter);
        }

        public IEnumerable<LoanApplication> GetLoanApplications(Func<LoanApplication, bool> filter)
        {
            return _unitOfWork.LoanApplicationRepository.GetAll(filter);
        }

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, bool> filter)
        {
            return _unitOfWork.TariffRepository.GetAll(filter);
        }

        public void DeleteTariffById(Guid id)
        {
            var tariff = _unitOfWork.TariffRepository.Get(t => t.Id.Equals(id));
            _unitOfWork.TariffRepository.Delete(tariff);
            _unitOfWork.Save();
        }

        public void DeleteLoanApplicationById(Guid id)
        {
            var loanApplication = _unitOfWork.LoanApplicationRepository.Get(la => la.Id.Equals(id));
            _unitOfWork.LoanApplicationRepository.Delete(loanApplication);
            _unitOfWork.Save();
        }

        public void ApproveLoanAppication(LoanApplication loanApplication)
        {
            _unitOfWork.LoanApplicationRepository.Approve(loanApplication);
            _unitOfWork.Save();
        }

        public void RejectLoanApplication(LoanApplication loanApplication)
        {
            _unitOfWork.LoanApplicationRepository.Reject(loanApplication);
            _unitOfWork.Save();
        }
    }
}

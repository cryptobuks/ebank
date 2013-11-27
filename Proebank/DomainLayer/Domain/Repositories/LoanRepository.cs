using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Domain.Repositories
{
    public class LoanRepository
    {
        private AbstractDataContext _context;
        private readonly TariffHelper _tariffHelper;
        private static readonly AccountType[] LoanAccountTypes = new[]
                {
                    AccountType.ContractService,
                    AccountType.GeneralDebt,
                    AccountType.Interest,
                    AccountType.OverdueGeneralDebt, 
                    AccountType.OverdueInterest,
                };

        public LoanRepository(AbstractDataContext context)
        {
            _context = context;
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
                _context.LoanApplications.AddOrUpdate(loanApplication);
                _context.SaveChanges();
            }
            // TODO: make it without exception (but it will fail test :) )
            else throw new ArgumentException("Loan application is not valid");
        }

        public void ConsiderLoanApplication(LoanApplication loanApplication, bool decision)
        {
            // TODO: change later
            if (decision)
            {
                loanApplication.Status = LoanApplicationStatus.Approved;
                _context.LoanApplications.AddOrUpdate(loanApplication);
            }
            else
            {
                loanApplication.Status = LoanApplicationStatus.Rejected;
                _context.LoanApplications.AddOrUpdate(loanApplication);
            }
        }



        public void UpsertTariff(Tariff tariff)
        {
            _context.Tariffs.AddOrUpdate(tariff);
            _context.SaveChanges();
        }

        public void UpsertLoan(Loan loan)
        {
            _context.Loans.AddOrUpdate(loan);
            _context.SaveChanges();
        }

        public void UpsertLoanApplication(LoanApplication loanApplication)
        {
            _context.LoanApplications.AddOrUpdate(loanApplication);
            _context.SaveChanges();
        }

        public bool CanLoanBeClosed(Loan loan)
        {
            return loan.Accounts.All(a => a.Balance == 0M);
        }

        public void CloseLoan(Loan loan)
        {
            loan.IsClosed = true;
            _context.Loans.AddOrUpdate(loan);
            _context.SaveChanges();
        }

        public IEnumerable<Loan> GetLoans(Func<Loan, bool> filter)
        {
            return _context.Loans.Where(filter);
        }

        public IEnumerable<LoanApplication> GetLoanApplications(Func<LoanApplication, bool> filter)
        {
            return _context.LoanApplications.Where(filter);
        }

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, bool> filter)
        {
            return _context.Tariffs.Where(filter);
        }

        public void DeleteTariffById(Guid id)
        {
            var tariff = _context.Tariffs.Single(t => t.Id.Equals(id));
            _context.Tariffs.Remove(tariff);
            _context.SaveChanges();
        }

        public void DeleteLoanApplicationById(Guid id)
        {
            var loanApplication = _context.LoanApplications.Single(la => la.Id.Equals(id));
            _context.LoanApplications.Remove(loanApplication);
            _context.SaveChanges();
        }

        public void ApproveLoanAppication(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.Approved;
            _context.LoanApplications.AddOrUpdate(loanApplication);
            _context.SaveChanges();
        }

        public void RejectLoanApplication(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.Rejected;
            _context.LoanApplications.AddOrUpdate(loanApplication);
            _context.SaveChanges();
        }
    }
}

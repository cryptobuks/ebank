using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Domain.Repositories
{
    public class LoanRepository
    {
        private DataContext Context;
        private readonly TariffHelper _tariffHelper;
        private static readonly AccountType[] LoanAccountTypes = new[]
                {
                    AccountType.ContractService,
                    AccountType.GeneralDebt,
                    AccountType.Interest,
                    AccountType.OverdueGeneralDebt, 
                    AccountType.OverdueInterest,
                };

        public LoanRepository(DataContext context)
        {
            Context = context;
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
                Context.LoanApplications.AddOrUpdate(loanApplication);
                Context.SaveChanges();
            }
            // TODO: make it without exception :)
            else throw new Exception("Loan application is not valid");
        }

        public void ConsiderLoanApplication(LoanApplication loanApplication, bool decision)
        {
            // TODO: change later
            if (decision)
            {
                loanApplication.Status = LoanApplicationStatus.Approved;
                Context.LoanApplications.AddOrUpdate(loanApplication);
            }
            else
            {
                loanApplication.Status = LoanApplicationStatus.Rejected;
                Context.LoanApplications.AddOrUpdate(loanApplication);
            }
        }



        public void UpsertTariff(Tariff tariff)
        {
            Context.Tariffs.AddOrUpdate(tariff);
            Context.SaveChanges();
        }

        public void UpsertLoan(Loan loan)
        {
            Context.Loans.AddOrUpdate(loan);
            Context.SaveChanges();
        }

        public void UpsertLoanApplication(LoanApplication loanApplication)
        {
            Context.LoanApplications.AddOrUpdate(loanApplication);
            Context.SaveChanges();
        }

        public bool CanLoanBeClosed(Loan loan)
        {
            return loan.Accounts.All(a => a.Balance == 0M);
        }

        public void CloseLoan(Loan loan)
        {
            loan.IsClosed = true;
            Context.Loans.AddOrUpdate(loan);
            Context.SaveChanges();
        }

        public IEnumerable<Loan> GetLoans(Func<Loan, bool> filter)
        {
            return Context.Loans.Where(filter);
        }

        public IEnumerable<LoanApplication> GetLoanApplications(Func<LoanApplication, bool> filter)
        {
            return Context.LoanApplications.Where(filter);
        }

        public IEnumerable<Tariff> GetTariffs(Func<Tariff, bool> filter)
        {
            return Context.Tariffs.Where(filter);
        }

        public void DeleteTariffById(Guid id)
        {
            var tariff = Context.Tariffs.Single(t => t.Id.Equals(id));
            Context.Tariffs.Remove(tariff);
            Context.SaveChanges();
        }

        public void DeleteLoanApplicationById(Guid id)
        {
            var loanApplication = Context.LoanApplications.Single(la => la.Id.Equals(id));
            Context.LoanApplications.Remove(loanApplication);
            Context.SaveChanges();
        }

        public void ApproveLoanAppication(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.Approved;
            Context.LoanApplications.AddOrUpdate(loanApplication);
            Context.SaveChanges();
        }

        public void RejectLoanApplication(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.Rejected;
            Context.LoanApplications.AddOrUpdate(loanApplication);
            Context.SaveChanges();
        }
    }
}

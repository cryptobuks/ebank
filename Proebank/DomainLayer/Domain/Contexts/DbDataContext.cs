using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Domain.Models.Users;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Contexts
{
    public class DbDataContext : DataContext
    {
        private IDbSet<Account> _accounts;
        private IDbSet<LoanApplication> _loanApplications;
        private IDbSet<Loan> _loans;
        private IDbSet<Tariff> _tariffs;
        private IDbSet<Calendar> _calendars;
        private IDbSet<Employee> _employees;
        private IDbSet<LoanHistory> _history;
        private IDbSet<PersonalData> _personalData;
        private IDbSet<IdentityUserLogin> _identityUserLogins;
        private IDbSet<IdentityUserRole> _identityUserRoles;

        public DbDataContext()
            : base("Proebank")
        {
        }

        public override IDbSet<Account> Accounts
        {
            get { return _accounts ?? (_accounts = Set<Account>()); }
            set { _accounts = value; }
        }

        public override IDbSet<LoanApplication> LoanApplications
        {
            get { return _loanApplications ?? (_loanApplications = Set<LoanApplication>()); }
            set { _loanApplications = value; }
        }

        public override IDbSet<Loan> Loans
        {
            get { return _loans ?? (_loans = Set<Loan>()); }
            set { _loans = value; }
        }

        public override IDbSet<Tariff> Tariffs
        {
            get { return _tariffs ?? (_tariffs = Set<Tariff>()); }
            set { _tariffs = value; }
        }

        public override IDbSet<Calendar> Calendars
        {
            get { return _calendars ?? (_calendars = Set<Calendar>()); }
            set { _calendars = value; }
        }

        public override IDbSet<Employee> Employees
        {
            get { return _employees ?? (_employees = Set<Employee>()); }
            set { _employees = value; }
        }

        public override IDbSet<LoanHistory> History
        {
            get { return _history ?? (_history = Set<LoanHistory>()); }
            set { _history = value; }
        }

        public override IDbSet<PersonalData> PersonalData
        {
            get { return _personalData ?? (_personalData = Set<PersonalData>()); }
            set { _personalData = value; }
        }

        public override IDbSet<IdentityUserLogin> IdentityUserLogins
        {
            get { return _identityUserLogins ?? (_identityUserLogins = Set<IdentityUserLogin>()); }
            set { _identityUserLogins = value; }
        }

        public override IDbSet<IdentityUserRole> IdentityUserRoles
        {
            get { return _identityUserRoles ?? (_identityUserRoles = Set<IdentityUserRole>()); }
            set { _identityUserRoles = value; }
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (DbEntityValidationException dbExc)
            {
                foreach (var validationError in dbExc.EntityValidationErrors
                    .SelectMany(validationErrors => validationErrors.ValidationErrors))
                {
                    Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                }
                throw;
            }
        }
    }
}

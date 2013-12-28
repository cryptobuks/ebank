using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Accounts;
using Domain.Models.Calendars;
using Domain.Models.Loans;
using Domain.Models.Users;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Domain.Contexts
{
    public class FakeDataContext : DataContext
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

        public override IDbSet<Account> Accounts
        {
            get { return _accounts ?? (_accounts = new FakeDbSet<Account>()); }
            set { _accounts = value; }
        }

        public override IDbSet<LoanApplication> LoanApplications
        {
            get { return _loanApplications ?? (_loanApplications = new FakeDbSet<LoanApplication>()); }
            set { _loanApplications = value; }
        }

        public override IDbSet<Loan> Loans
        {
            get { return _loans ?? (_loans = new FakeDbSet<Loan>()); }
            set { _loans = value; }
        }

        public override IDbSet<Tariff> Tariffs
        {
            get { return _tariffs ?? (_tariffs = new FakeDbSet<Tariff>()); }
            set { _tariffs = value; }
        }

        public override IDbSet<Calendar> Calendars
        {
            get { return _calendars ?? (_calendars = new FakeDbSet<Calendar>()); }
            set { _calendars = value; }
        }

        public override IDbSet<Employee> Employees
        {
            get { return _employees ?? (_employees = new FakeDbSet<Employee>()); }
            set { _employees = value; }
        }

        public override IDbSet<LoanHistory> History
        {
            get { return _history ?? (_history = new FakeDbSet<LoanHistory>()); }
            set { _history = value; }
        }

        public override IDbSet<PersonalData> PersonalData
        {
            get { return _personalData ?? (_personalData = new FakeDbSet<PersonalData>()); }
            set { _personalData = value; }
        }

        public override IDbSet<IdentityUserLogin> IdentityUserLogins
        {
            get { return _identityUserLogins ?? (_identityUserLogins = new FakeDbSet<IdentityUserLogin>()); }
            set { _identityUserLogins = value; }
        }

        public override IDbSet<IdentityUserRole> IdentityUserRoles
        {
            get { return _identityUserRoles ?? (_identityUserRoles = new FakeDbSet<IdentityUserRole>()); }
            set { _identityUserRoles = value; }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Contexts.Factories;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Domain.Repositories
{
    public interface IAccountRepository : IRepository<Account>
    {
    }
    public interface IEntryRepository : IRepository<Entry>
    {
    }
    public interface ILoanRepository : IRepository<Loan>
    {
    }
    public interface ILoanApplicationRepository : IRepository<LoanApplication>
    {
    }

    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(IDataContextFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public class EntryRepository : RepositoryBase<Entry>, IEntryRepository
    {
        public EntryRepository(IDataContextFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }


    public class LoanRepository : RepositoryBase<Loan>, ILoanRepository
    {
        public LoanRepository(IDataContextFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }

    public class LoanApplicationRepository : RepositoryBase<LoanApplication>, ILoanApplicationRepository
    {
        public LoanApplicationRepository(IDataContextFactory databaseFactory)
            : base(databaseFactory)
        {
        }
    }
}

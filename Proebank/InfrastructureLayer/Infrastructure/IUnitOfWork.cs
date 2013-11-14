using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Accounts;
using Domain.Models.Loans;

namespace Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IAccountRepository AccountRepository { get; }
        ILoanRepository LoanRepository { get; }
        ILoanApplicationRepository LoanApplicationRepository { get; }
        ITariffRepository TariffRepository { get; }

        void Save();
    }
}

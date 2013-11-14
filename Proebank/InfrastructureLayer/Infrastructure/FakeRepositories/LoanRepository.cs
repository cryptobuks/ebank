using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;

namespace Infrastructure.FakeRepositories
{
    class LoanRepository : AbstractRepository<Loan, Guid>, ILoanRepository
    {
        public LoanRepository(object _isDisposedIndicator) : base(_isDisposedIndicator) { }
    }
}

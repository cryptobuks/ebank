using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;

namespace Infrastructure.FakeRepositories
{
    class LoanApplicationRepository : AbstractRepository<LoanApplication, long>, ILoanApplicationRepository
    {
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Loans
{
    public interface ILoanApplicationRepository : IRepository<LoanApplication, long>
    {
    }
}

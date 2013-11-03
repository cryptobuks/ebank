using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Loans;

namespace Infrastructure.FakeRepositories
{
    class LoanRepository : ILoanRepository
    {
        public Loan Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public Loan Get(Func<Loan, bool> filter)
        {
            throw new NotImplementedException();
        }

        public IList<Loan> GetAll()
        {
            throw new NotImplementedException();
        }

        public IList<Loan> GetAll(Func<Loan, bool> filter)
        {
            throw new NotImplementedException();
        }

        public void SaveOrUpdate(params Loan[] entities)
        {
            throw new NotImplementedException();
        }

        public Loan Delete(Loan entity)
        {
            throw new NotImplementedException();
        }
    }
}

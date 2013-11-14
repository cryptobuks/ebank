using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Loans;

namespace Infrastructure.FakeRepositories
{
    class LoanApplicationRepository : AbstractRepository<LoanApplication, long>, ILoanApplicationRepository
    {
        public LoanApplicationRepository(object _isDisposedIndicator) : base(_isDisposedIndicator) { }

        public void Approve(LoanApplication loanApplication)
        {
            var entity = Get(la => la.Id.Equals(loanApplication.Id));
            if (entity != null)
            {
                entity.Status = LoanApplicationStatus.Approved;
            }
        }

        // TODO: add reason text and/or enum
        public void Reject(LoanApplication loanApplication)
        {
            var entity = Get(la => la.Id.Equals(loanApplication.Id));
            if (entity != null)
            {
                entity.Status = LoanApplicationStatus.Rejected;
            }
        }

        public void Contract(LoanApplication loanApplication)
        {
            var entity = Get(la => la.Id.Equals(loanApplication.Id));
            if (entity != null && entity.Status == LoanApplicationStatus.Approved)
            {
                entity.Status = LoanApplicationStatus.Contracted;
                entity.TimeContracted = DateTime.UtcNow;
            }
        }
    }
}

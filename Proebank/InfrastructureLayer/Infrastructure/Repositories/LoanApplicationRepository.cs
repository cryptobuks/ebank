using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Loans;

namespace Infrastructure.Repositories
{
    // TODO: don't create datacontext every time and try use IQueryable instead of IList
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        private DataContext _ctx;
        DataContext ctx
        {
            get
            {
                // TODO: need lock()
                if (_ctx == null)
                    _ctx = new DataContext();
                return _ctx;
            }
        }
        public LoanApplication Get(Func<LoanApplication, bool> filter)
        {
            return ctx.LoanApplications
                .AsQueryable()
                .First(filter);
        }

        public IList<LoanApplication> GetAll()
        {
            return ctx.LoanApplications.ToList();
        }

        public IList<LoanApplication> GetAll(Func<LoanApplication, bool> filter)
        {
            return ctx.LoanApplications
                .AsQueryable()
                .Where(filter)
                .ToList();
        }

        public void SaveOrUpdate(params LoanApplication[] entities)
        {
            ctx.LoanApplications.AddOrUpdate(entities);
            ctx.SaveChanges();
        }

        public LoanApplication Delete(LoanApplication entity)
        {
            var removedApplication = ctx.LoanApplications.Remove(entity);
            ctx.SaveChanges();
            return removedApplication;
        }

        public void Approve(LoanApplication entity)
        {
            entity.Status = LoanApplicationStatus.Approved;
            SaveOrUpdate(entity);
        }

        public void Reject(LoanApplication entity)
        {
            entity.Status = LoanApplicationStatus.Rejected;
            SaveOrUpdate(entity);
        }

        public void Contract(LoanApplication entity)
        {
            entity.TimeContracted = DateTime.UtcNow;
            entity.Status = LoanApplicationStatus.Contracted;
            SaveOrUpdate(entity);
        }

        public void Dispose(LoanApplication loanApplication)
        {
            _ctx.Dispose();
            _ctx = null;
        }
    }
}

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
    public class LoanApplicationRepository : ILoanApplicationRepository
    {
        private readonly DataContext _context;

        public LoanApplicationRepository(DataContext context)
        {
            // TODO: Complete member initialization
            this._context = context;
        }
        public LoanApplication Get(Func<LoanApplication, bool> filter)
        {
            return _context.LoanApplications
                .AsQueryable()
                .First(filter);
        }

        public IQueryable<LoanApplication> GetAll()
        {
            return _context.LoanApplications.AsQueryable();
        }

        public IEnumerable<LoanApplication> GetAll(Func<LoanApplication, bool> filter)
        {
            return _context.LoanApplications
                .AsQueryable()
                .Where(filter);
        }

        public void SaveOrUpdate(params LoanApplication[] entities)
        {
            _context.LoanApplications.AddOrUpdate(entities);
            _context.SaveChanges();
        }

        public LoanApplication Delete(LoanApplication entity)
        {
            var removedApplication = _context.LoanApplications.Remove(entity);
            _context.SaveChanges();
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
    }
}

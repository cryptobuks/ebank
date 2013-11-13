using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Domain.Models.Loans;

namespace Infrastructure.Repositories
{
    // TODO: try to generalize more, using DbSet<T> everywhere
    public class LoanRepository : ILoanRepository
    {
        DataContext ctx = new DataContext();
        public Loan Get(Func<Loan, bool> filter)
        {            
            return ctx.Loans
                .AsQueryable()
                .First(filter);
        }

        public IList<Loan> GetAll()
        {
            return ctx.Loans.ToList();
        }

        public IList<Loan> GetAll(Func<Loan, bool> filter)
        {
            return ctx.Loans
                .AsQueryable()
                .Where(filter)
                .ToList();
        }

        public void SaveOrUpdate(params Loan[] entities)
        {
            ctx.Loans.AddOrUpdate(entities);
            ctx.SaveChanges();
        }

        public Loan Delete(Loan entity)
        {
            var removedLoan = ctx.Loans.Remove(entity);
            ctx.SaveChanges();
            return removedLoan;
        }
    }
}

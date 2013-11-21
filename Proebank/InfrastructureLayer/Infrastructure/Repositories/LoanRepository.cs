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
        private readonly DataContext _context;

        public LoanRepository(DataContext context)
        {
            // TODO: Complete member initialization
            _context = context;
        }
        public Loan Get(Func<Loan, bool> filter)
        {
            return _context.Loans
                .AsQueryable()
                .First(filter);
        }

        public IQueryable<Loan> GetAll()
        {
            return _context.Loans.AsQueryable();
        }

        public IEnumerable<Loan> GetAll(Func<Loan, bool> filter)
        {
            return _context.Loans
                .AsQueryable()
                .Where(filter);
        }

        public void Upsert(params Loan[] entities)
        {
            _context.Loans.AddOrUpdate(entities);
        }

        public Loan Delete(Loan entity)
        {
            var removedLoan = _context.Loans.Remove(entity);
            return removedLoan;
        }
    }
}

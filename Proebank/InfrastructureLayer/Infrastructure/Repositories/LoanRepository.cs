using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Domain.Models;
using RepositoriesContracts;

namespace Infrastructure.Repositories
{
    // TODO: try to generalize more, using DbSet<T> everywhere
    class LoanRepository : ILoanRepository
    {
        public Loan Get(Guid id)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans.First(loan => loan.Id == id);
            }
        }

        public IList<Loan> GetAll()
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans.ToList();
            }
        }

        public IList<Loan> GetAll(Func<Loan, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans
                    .Where(loan => filter(loan))
                    .ToList();
            }
        }

        public Loan Get(Func<Loan, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans
                    .First(a => filter(a));
            }
        }

        public void SaveOrUpdate(params Loan[] entities)
        {
            using (var ctx = new DataContext())
            {
                ctx.Loans.AddOrUpdate(entities);
            }
        }

        public Loan Delete(Loan entity)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans.Remove(entity);
            }
        }
    }
}

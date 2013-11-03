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
        public LoanModel Get(Guid id)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans.First(loan => loan.Id == id);
            }
        }

        public IList<LoanModel> GetAll()
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans.ToList();
            }
        }

        public IList<LoanModel> FindAll(Func<LoanModel, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans
                    .Where(loan => filter(loan))
                    .ToList();
            }
        }

        public LoanModel FindFirst(Func<LoanModel, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans
                    .First(a => filter(a));
            }
        }

        public void SaveOrUpdate(params LoanModel[] entities)
        {
            using (var ctx = new DataContext())
            {
                ctx.Loans.AddOrUpdate(entities);
            }
        }

        public LoanModel Delete(LoanModel entity)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Loans.Remove(entity);
            }
        }
    }
}

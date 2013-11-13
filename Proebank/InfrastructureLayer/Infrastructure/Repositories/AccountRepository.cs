using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Domain.Models.Accounts;

namespace Infrastructure.Repositories
{
    // TODO: don't create db context for every method call
    public class AccountRepository : IAccountRepository
    {
        public IList<Account> GetAll()
        {
            var ctx = new DataContext();
            return ctx.Accounts.ToList();
        }

        public IList<Account> GetAll(Func<Account, bool> filter)
        {
            var ctx = new DataContext();
            return ctx.Accounts
                .AsQueryable()
                .Where(filter)
                .ToList();
        }

        public Account Get(Func<Account, bool> filter)
        {
            var ctx = new DataContext();
            return ctx.Accounts
                .AsQueryable()
                .First(filter);
        }

        public void SaveOrUpdate(params Account[] entities)
        {
            var ctx = new DataContext();
            ctx.Accounts.AddOrUpdate(entities);
            ctx.SaveChanges();
        }

        public Account Delete(Account entity)
        {
            using (var ctx = new DataContext())
            {
                var removedAccount = ctx.Accounts.Remove(entity);
                ctx.SaveChanges();
                return removedAccount;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using Domain.Models;
using RepositoriesContracts;

namespace Infrastructure.Repositories
{
    // TODO: don't create db context for every method call
    public class AccountRepository : IAccountRepository
    {
        public AccountModel Get(Guid id)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts.First(acc => acc.Id == id);
            }
        }

        public IList<AccountModel> GetAll()
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts.ToList();
            }
        }

        public IList<AccountModel> FindAll(Func<AccountModel, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts
                    .Where(ac => filter(ac))
                    .ToList();
            }
        }

        public AccountModel FindFirst(Func<AccountModel, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts
                    .First(a => filter(a));
            }
        }

        public void SaveOrUpdate(params AccountModel[] entities)
        {
            using (var ctx = new DataContext())
            {
                ctx.Accounts.AddOrUpdate(entities);
            }
        }

        public AccountModel Delete(AccountModel entity)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts.Remove(entity);
            }
        }
    }
}

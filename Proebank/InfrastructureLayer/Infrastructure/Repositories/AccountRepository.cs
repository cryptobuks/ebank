﻿using System;
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
        public Account Get(Guid id)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts.First(acc => acc.Id == id);
            }
        }

        public IList<Account> GetAll()
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts.ToList();
            }
        }

        public IList<Account> GetAll(Func<Account, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts
                    .Where(ac => filter(ac))
                    .ToList();
            }
        }

        public Account Get(Func<Account, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts
                    .First(a => filter(a));
            }
        }

        public void SaveOrUpdate(params Account[] entities)
        {
            using (var ctx = new DataContext())
            {
                ctx.Accounts.AddOrUpdate(entities);
            }
        }

        public Account Delete(Account entity)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts.Remove(entity);
            }
        }
    }
}

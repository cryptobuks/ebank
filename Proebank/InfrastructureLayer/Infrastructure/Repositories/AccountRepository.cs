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
        private readonly DataContext _context;

        public AccountRepository(DataContext context)
        {
            // TODO: Complete member initialization
            this._context = context;
        }

        public IList<Account> GetAll()
        {
            return _context.Accounts.ToList();
        }

        public IList<Account> GetAll(Func<Account, bool> filter)
        {
            return _context.Accounts
                .AsQueryable()
                .Where(filter)
                .ToList();
        }

        public Account Get(Func<Account, bool> filter)
        {
            return _context.Accounts
                .AsQueryable()
                .First(filter);
        }

        public void SaveOrUpdate(params Account[] entities)
        {
            _context.Accounts.AddOrUpdate(entities);
            _context.SaveChanges();
        }

        public Account Delete(Account entity)
        {
            var removedAccount = _context.Accounts.Remove(entity);
            _context.SaveChanges();
            return removedAccount;
        }
    }
}

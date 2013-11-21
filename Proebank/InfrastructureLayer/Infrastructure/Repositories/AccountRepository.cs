using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using Domain.Models.Accounts;

namespace Infrastructure.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        private readonly DataContext _context;

        public AccountRepository(DataContext context)
        {
            _context = context;
        }

        public IQueryable<Account> GetAll()
        {
            return _context.Accounts.AsQueryable();
        }

        public IEnumerable<Account> GetAll(Func<Account, bool> filter)
        {
            return _context.Accounts
                .AsQueryable()
                .Where(filter);
        }

        public Account Get(Func<Account, bool> filter)
        {
            return _context.Accounts
                .AsQueryable()
                .First(filter);
        }

        public void Upsert(params Account[] entities)
        {
            _context.Accounts.AddOrUpdate(entities);
        }

        public Account Delete(Account entity)
        {
            var removedAccount = _context.Accounts.Remove(entity);
            return removedAccount;
        }
    }
}

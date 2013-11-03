using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models.Accounts;

namespace Infrastructure.FakeRepositories
{
    class AccountRepository : IAccountRepository
    {
        private readonly List<Account> _accounts;

        public AccountRepository()
        {
            _accounts = new List<Account>();
        }

        public Account Get(Guid id)
        {
            return Get(a => a.Id == id);
        }

        public IList<Account> GetAll()
        {
            return _accounts;
        }

        public IList<Account> GetAll(Func<Account, bool> filter)
        {
            return _accounts.Where(filter).ToList();
        }

        public Account Get(Func<Account, bool> filter)
        {
            return _accounts.FirstOrDefault();
        }

        public void SaveOrUpdate(params Account[] entities)
        {
            foreach (var entity in entities)
            {
                var id = entity.Id;
                var acc = Get(a => a.Id == id);
                if (acc == null)
                {
                    _accounts.Add(entity);
                    acc = entity;
                }
                else
                {
                    // TODO: complete; extract method
                    acc.Entries = entity.Entries;
                    acc.DateOpened = entity.DateOpened;
                    acc.Number = entity.Number;
                }
            }
        }

        public Account Delete(Account entity)
        {
            var removalSucceeded = _accounts.Remove(entity);
            return removalSucceeded ? entity : null;
        }
    }
}

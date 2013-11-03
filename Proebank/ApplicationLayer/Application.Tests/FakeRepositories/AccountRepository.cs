using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Models;
using RepositoriesContracts;

namespace Application.Tests.FakeRepositories
{
    class AccountRepository : IAccountRepository
    {
        private readonly List<AccountModel> _accounts;

        public AccountRepository()
        {
            _accounts = new List<AccountModel>();
        }

        public AccountModel Get(Guid id)
        {
            return FindFirst(a => a.Id == id);
        }

        public IList<AccountModel> GetAll()
        {
            return _accounts;
        }

        public IList<AccountModel> FindAll(Func<AccountModel, bool> filter)
        {
            return _accounts.Where(filter).ToList();
        }

        public AccountModel FindFirst(Func<AccountModel, bool> filter)
        {
            return _accounts.FirstOrDefault();
        }

        public void SaveOrUpdate(params AccountModel[] entities)
        {
            foreach (var entity in entities)
            {
                var id = entity.Id;
                var acc = FindFirst(a => a.Id == id);
                if (acc == null)
                {
                    _accounts.Add(entity);
                    acc = entity;
                }
                else
                {
                    acc.Balance = entity.Balance;
                    acc.CreationDate = entity.CreationDate;
                    acc.Employee = entity.Employee;
                    acc.Number = entity.Number;
                }
            }
        }

        public AccountModel Delete(AccountModel entity)
        {
            var removalSucceeded = _accounts.Remove(entity);
            return removalSucceeded ? entity : null;
        }
    }
}

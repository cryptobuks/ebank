using System;
using System.Collections.Generic;
using System.Linq;
using CrossCutting.Interfaces;
using RepositoriesContracts;

namespace InfrastructureLayer.Repositories
{
    public class AccountRepository : IAccountRepository
    {
        public IAccount Get(Guid id)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts.First(acc => acc.Id == id);
            }
        }

        public IList<IAccount> GetAll()
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts
                    .Select(a => a as IAccount)
                    .ToList();
            }
        }

        public IAccount SaveOrUpdate(IAccount entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(IAccount entity)
        {
            throw new NotImplementedException();
        }


        public IList<IAccount> FindAll(Func<IAccount, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts
                    .Select(a => a as IAccount)
                    .Where(ac => filter(ac))
                    .ToList();
            }
        }

        public IAccount FindOne(Func<IAccount, bool> filter)
        {
            using (var ctx = new DataContext())
            {
                return ctx.Accounts
                    .Single(a => filter(a));
            }
        }
    }
}

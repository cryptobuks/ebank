using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CrossCutting.Interfaces;
using InfrastructureLayer.Entities;
using RepositoriesContracts;

namespace InfrastructureLayer.Repositories
{
    public class AccountRepository : IAccountRepository
    {

        public IAccount Build(string number, IEmployee employee)
        {
            IAccount acc = new Account
            {
                Balance = 0,
                CreationDate = DateTime.Now,
                Employee = employee,
                Id = Guid.NewGuid(),
                Number = number // TODO: replace with converter from account type to number
            };
            return acc;
        }

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

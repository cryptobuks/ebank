using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Tests.TmpEntities;
using CrossCutting.Interfaces;
using RepositoriesContracts;

namespace Application.Tests.FakeRepositories
{
    class AccountRepository : IAccountRepository
    {
        private readonly List<IAccount> _accounts;

        public AccountRepository()
        {
            _accounts = new List<IAccount>();
        }

        public IAccount Get(Guid id)
        {
            return FindOne(a => a.Id == id);
        }

        public IList<IAccount> GetAll()
        {
            return _accounts;
        }

        public IList<IAccount> FindAll(Func<IAccount, bool> filter)
        {
            return _accounts.Where(filter).ToList();
        }

        public IAccount FindOne(Func<IAccount, bool> filter)
        {
            return _accounts.SingleOrDefault();
        }

        public IAccount SaveOrUpdate(IAccount entity)
        {
            var acc = FindOne(a => a.Id == entity.Id);
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
            return acc;
        }

        public void Delete(IAccount entity)
        {
            _accounts.Remove(entity);
        }

        // TODO: улучшить модель - добавить класс для выбора номера счёта и остальные TODO писать на английском. Или не писать :)
        // TODO: убрать этот метод или изменить сигнатуру, в т.ч. и создание гуида снаружи
        public IAccount Build(string number, IEmployee employee)
        {
            return new Account(Guid.NewGuid()) {Balance = 0, CreationDate = DateTime.Now, Employee = employee, Number = number};
        }
    }
}

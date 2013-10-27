using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class AccountModel
    {
        private readonly IAccount _account;

        public AccountModel(IAccount acc)
        {
            _account = acc;
        }

        // TODO: подумать, как можно избавиться от этого свойства
        public IAccount Entity { get { return _account; } }

        public Guid Id
        {
            get { return _account.Id; }
        }

        public string Number
        {
            get { return _account.Number; }
            private set { _account.Number = value; }
        }

        public decimal Balance
        {
            get { return _account.Balance; } 
            private set { _account.Balance = value; }
        }

        public DateTime CreationDate
        {
            get { return _account.CreationDate; }
            private set { _account.CreationDate = value; }
        }

        public IEmployee Employee
        {
            get { return _account.Employee; }
            private set { _account.Employee = value; } 
        }
    }
}

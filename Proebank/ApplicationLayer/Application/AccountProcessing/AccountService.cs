using DomainLayer.Models;
using DomainLayer.RepositoriesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.AccountProcessing
{
    class AccountService
    {
        private readonly IAccountRepository _repository;

        public void CommitTransaction(TransactionModel transaction)
        {
            throw new System.NotImplementedException();
        }

        public TransactionModel FindTransactionById()
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<TransactionModel> FindTransactionsBy()
        {
            throw new System.NotImplementedException();
        }

        public AccountModel CreateAccount()
        {
            throw new System.NotImplementedException();
        }

        public void CloseAccount()
        {
            throw new System.NotImplementedException();
        }
    }
}

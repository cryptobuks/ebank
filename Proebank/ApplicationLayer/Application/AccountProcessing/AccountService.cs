using CrossCutting.Interfaces;
using DomainLayer.Models;
using RepositoriesContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationLayer.AccountProcessing
{
    public class AccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository accountRepository)
        {
            _repository = accountRepository;
        }

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
            // TODO: Полная чушь, надо переписать
            var iAcc = _repository.Build("3819xxx", null);
            var acc = new AccountModel(iAcc);
            var savedEntity = _repository.SaveOrUpdate(acc.Entity);
            return savedEntity != null ? acc: null;
        }

        public void CloseAccount()
        {
            throw new System.NotImplementedException();
        }
    }
}

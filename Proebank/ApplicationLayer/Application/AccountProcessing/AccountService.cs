using Domain.Models;
using RepositoriesContracts;
using System;
using System.Collections.Generic;

namespace Application.AccountProcessing
{
    public class AccountService
    {
        private readonly IAccountRepository _repository;

        public AccountService(IAccountRepository accountRepository)
        {
            _repository = accountRepository;
        }

        public AccountModel CreateAccount()
        {
            // TODO: Полная чушь, надо переписать
            var acc = new AccountModel { Id = Guid.NewGuid(), Number = "3819xxx" };
            _repository.SaveOrUpdate(acc);
            return acc;
        }

        public void CloseAccount()
        {
            throw new System.NotImplementedException();
        }
    }
}

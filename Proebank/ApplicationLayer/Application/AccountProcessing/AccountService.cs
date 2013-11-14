using Domain.Enums;
using Domain.Models;
using Domain.Models.Accounts;
using Infrastructure;
using System;
using System.Collections.Generic;

namespace Application.AccountProcessing
{
    public class AccountService
    {
        private IUnitOfWork _unitOfWork;
        //// TODO: создать его в методе Seed базы; для каждой валюты свой!
        //public static Account BankAccount { get; private set; }

        public AccountService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            //BankAccount = _repository.Get(acc => 
            //    acc.Type == AccountType.BankBalance &&);
        }

        public Account CreateAccount(Currency currency, AccountType accountType)
        {
            var acc = new Account()
            {
                Currency = currency,
                DateOpened = DateTime.UtcNow,
                Type = accountType,
            };
            _unitOfWork.AccountRepository.SaveOrUpdate(acc);
            return acc;
        }

        public void AddEntry(Account account, Entry entry)
        {
            if (account == null)
                throw new ArgumentNullException("account");
            if (entry == null)
                throw new ArgumentNullException("entry");
            if (account.Currency != entry.Currency)
                throw new ArgumentException("Currencies are not equal");
            account.Entries.Add(entry);
            _unitOfWork.AccountRepository.SaveOrUpdate(account);
        }

        public void CloseAccount(Account account)
        {
            if (account.IsClosed)
            {
                throw new ArgumentException("Account is already closed");
            }
            else
            {
                account.IsClosed = true;
                account.DateClosed = DateTime.UtcNow;
                _unitOfWork.AccountRepository.SaveOrUpdate(account);
            }
        }
    }
}

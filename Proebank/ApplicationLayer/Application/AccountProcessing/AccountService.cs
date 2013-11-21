using System.Data.Entity.Migrations;
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
        private DataContext Context { get; set; }
        //// TODO: создать его в методе Seed базы; для каждой валюты свой!
        //public static Account BankAccount { get; private set; }

        public AccountService(DataContext context)
        {
            Context = context;
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
            Context.Accounts.AddOrUpdate(acc);
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
            Context.Accounts.AddOrUpdate(account);
            Context.SaveChanges();
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
                Context.Accounts.AddOrUpdate(account);
            }
        }
    }
}

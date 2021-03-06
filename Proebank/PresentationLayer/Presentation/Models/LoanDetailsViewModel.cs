﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel.Channels;
using Domain.Enums;
using Domain.Models.Accounts;
using Domain.Models.Customers;
using Domain.Models.Loans;

namespace Presentation.Models
{
    public class LoanDetailsViewModel
    {
        public Guid Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string TariffName { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int Term { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<Tuple<Account, Entry>> Entries { get; set; }
        public Customer Customer { get; set; }
        public bool CanBeClosed { get; set; }
        public bool IsClosed { get; set; }

        public LoanDetailsViewModel(Loan loan)
        {
            Id = loan.Id;
            var application = loan.Application;
            ApplicationId = application.Id;
            TariffName = application.Tariff.Name;
            Amount = application.LoanAmount;
            Currency = application.Currency;
            Term = application.Term;
            Accounts = loan.Accounts;
            IsClosed = loan.IsClosed;
            Entries = loan.Accounts.SelectMany(acc => acc.Entries.Select(e => new Tuple<Account, Entry>(acc, e))).OrderBy(e => e.Item2.Date);
        }
    }
}
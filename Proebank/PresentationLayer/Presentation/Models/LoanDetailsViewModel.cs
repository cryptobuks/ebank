using System;
using System.Collections.Generic;
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
        public string TariffName { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public int Term { get; set; }
        public IEnumerable<Account> Accounts { get; set; }
        public IEnumerable<Tuple<Account, Entry>> Entries { get; set; }
        public Customer Customer { get; set; }

        public LoanDetailsViewModel(Loan loan)
        {
            var application = loan.Application;
            TariffName = application.Tariff.Name;
            Amount = application.LoanAmount;
            Currency = application.Currency;
            Term = application.Term;
            Accounts = loan.Accounts;
            Entries = loan.Accounts.SelectMany(acc => acc.Entries.Select(e => new Tuple<Account, Entry>(acc, e))).OrderBy(e => e.Item2.Date);
        }
    }
}
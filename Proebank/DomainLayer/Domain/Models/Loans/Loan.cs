using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Models.Accounts;
using Domain.Models.Users;

namespace Domain.Models.Loans
{
    public class Loan
    {
        public Loan()
        {
            Id = Guid.NewGuid();
        }

        public readonly Guid Id;

        public LoanApplication Application { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public PaymentSchedule PaymentSchedule { get; set; }

        public bool IsClosed { get; set; }
    }
}

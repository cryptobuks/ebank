using System;
using System.Collections.Generic;
using Domain.Models.Accounts;
using Domain.Models.Users;

namespace Domain.Models.Loans
{
    public class Loan
    {
        public Guid Id { get; set; }

        public LoanApplication Application { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public PaymentSchedule PaymentSchedule { get; set; }
    }
}

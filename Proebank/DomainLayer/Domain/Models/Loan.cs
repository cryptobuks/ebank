using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class Loan
    {
        public Guid Id { get; set; }

        public LoanApplication Application { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public PaymentSchedule PaymentSchedule { get; set; }

        public Employee ConcludedBy { get; set; }
    }
}

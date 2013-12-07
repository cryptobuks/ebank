using System;
using System.Collections.Generic;
using System.ComponentModel;
using Domain.Models.Accounts;
using Domain.Models.Customers;

namespace Domain.Models.Loans
{
    public class Loan : Entity
    {
        public virtual Customer Customer { get; set; }

        public virtual LoanApplication Application { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public virtual PaymentSchedule PaymentSchedule { get; set; }

        [DisplayName("Is Closed")]
        public bool IsClosed { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public Guid Id { get; set; }

        public virtual LoanApplication Application { get; set; }

        public virtual ICollection<Account> Accounts { get; set; }

        public virtual PaymentSchedule PaymentSchedule { get; set; }

        [DisplayName("Is Closed")]
        public bool IsClosed { get; set; }
    }
}

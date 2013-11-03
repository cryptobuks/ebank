using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class LoanModel
    {
        public Guid Id { get; set; }

        public LoanApplicationModel Application { get; set; }

        public IEnumerable<AccountModel> Accounts { get; set; }

        public PaymentScheduleModel PaymentSchedule { get; set; }
    }
}

using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class LoanModel
    {
        private ILoan _loan;

        public Guid Id { get; set; }

        public LoanApplicationModel Application { get; set; }

        public IEnumerable<AccountModel> Accounts { get; set; }

        public PaymentScheduleModel PaymentSchedule { get; set; }
    }
}

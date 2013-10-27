using CrossCutting.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class LoanModel : ILoan
    {
        public Guid Id { get; set; }

        public ILoanApplication Application { get; set; }

        public IEnumerable<IAccount> Accounts { get; set; }

        public IPaymentSchedule PaymentSchedule { get; set; }
    }
}

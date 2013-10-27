using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface ILoan
    {
        Guid Id { get; set; }

        ILoanApplication Application { get; set; }

        IEnumerable<IAccount> Accounts { get; set; }

        IPaymentSchedule PaymentSchedule { get; set; }
    }
}

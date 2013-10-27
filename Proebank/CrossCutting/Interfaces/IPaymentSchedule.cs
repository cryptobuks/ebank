using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrossCutting.Interfaces
{
    public interface IPaymentSchedule
    {
        long Id
        {
            get;
            set;
        }

        IEnumerable<IPayment> Payments
        {
            get;
            set;
        }

        Guid AddPayment();
    }
}

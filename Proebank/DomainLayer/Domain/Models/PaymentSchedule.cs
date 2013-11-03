using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class PaymentSchedule
    {
        public long Id { get; set; }

        public IEnumerable<Payment> Payments { get; set; }

        public Guid AddPayment()
        {
            throw new NotImplementedException();
        }
    }
}

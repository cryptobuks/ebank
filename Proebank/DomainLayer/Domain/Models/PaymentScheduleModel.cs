using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class PaymentScheduleModel
    {
        public long Id { get; set; }

        public IEnumerable<PaymentModel> Payments { get; set; }

        public Guid AddPayment()
        {
            throw new NotImplementedException();
        }
    }
}

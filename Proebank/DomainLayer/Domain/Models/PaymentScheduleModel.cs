using System;
using System.Collections.Generic;

namespace DomainLayer.Models
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

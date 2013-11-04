using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Loans
{
    public class PaymentSchedule
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public IEnumerable<Payment> Payments { get; set; }

        public Guid AddPayment()
        {
            throw new NotImplementedException();
        }
    }
}

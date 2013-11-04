using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Models.Loans
{
    public class PaymentSchedule
    {
        public PaymentSchedule()
        {
            Payments = new Collection<Payment>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public Collection<Payment> Payments { get; private set; }

        public void AddPayment(Payment payment)
        {
            Payments.Add(payment);
        }
    }
}

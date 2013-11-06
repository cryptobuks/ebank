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
            Id = Guid.NewGuid();
            Payments = new Collection<Payment>();
        }

        public Guid Id { get; set; }

        public Collection<Payment> Payments { get; private set; }

        public void AddPayment(Payment payment)
        {
            Payments.Add(payment);
        }
    }
}

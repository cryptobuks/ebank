using System;
using System.Collections.ObjectModel;

namespace Domain.Models.Loans
{
    public class PaymentSchedule : Entity
    {
        public PaymentSchedule()
        {
            Payments = new Collection<Payment>();
        }

        public virtual Collection<Payment> Payments { get; private set; }

        public void AddPayment(Payment payment)
        {
            Payments.Add(payment);
        }
    }
}

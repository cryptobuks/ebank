using System;
using System.Collections.ObjectModel;
using System.Linq;

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

        public decimal MainDebtOverallAmount { get { return Payments.Sum(p => p.MainDebtAmount); } }
        public decimal InterestOverallAmount { get { return Payments.Sum(p => p.AccruedInterestAmount); } }
    }
}

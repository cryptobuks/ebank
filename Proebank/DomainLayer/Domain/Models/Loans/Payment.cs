using System;

namespace Domain.Models.Loans
{
    public class Payment
    {
        public Payment()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public DateTime ShouldBePaidBefore { get; set; }

        public decimal Amount { get; set; }

        public bool SameIdentityAs(Payment other)
        {
            return other != null && other.Id.Equals(Id);
        }
    }
}

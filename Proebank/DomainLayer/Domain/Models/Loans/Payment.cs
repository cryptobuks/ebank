using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Shared;

namespace Domain.Models.Loans
{
    public class Payment : IEntity<Payment>
    {
        public Guid Id { get; set; }

        public DateTime ShouldBePaidBefore { get; set; }

        public decimal Amount { get; set; }

        public bool SameIdentityAs(Payment other)
        {
            return other != null && other.Id.Equals(Id);
        }
    }
}

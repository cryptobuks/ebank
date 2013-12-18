using System;

namespace Domain.Models.Loans
{
    public class Payment : Entity
    {
        public DateTime? ShouldBePaidBefore { get; set; }
        public DateTime? AccruedOn { get; set; }
        public bool IsPaid { get; set; }
        public decimal MainDebtAmount { get; set; }
        public decimal AccruedInterestAmount { get; set; }
        public decimal OverdueMainDebtAmount { get; set; }
        public decimal OverdueInterestAmount { get; set; }

        public decimal Amount
        {
            get
            {
                return MainDebtAmount + AccruedInterestAmount + OverdueMainDebtAmount + OverdueInterestAmount;
            }
        }
    }
}

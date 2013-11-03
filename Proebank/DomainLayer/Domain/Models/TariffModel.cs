using System;

namespace Domain.Models
{
    public class TariffModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public decimal InterestRate { get; set; }

        public decimal MinAmount { get; set; }

        public decimal MaxAmount { get; set; }

        public DateTime CreationDate { get; set; }

        public DateTime EndDate { get; set; }

        public int MinTerm { get; set; }

        public int MaxTerm { get; set; }

        public ushort MinAge { get; set; }

        public ushort? MaxAge { get; set; }

        public decimal InitialFee { get; set; }

        public bool IsGuarantorNeeded { get; set; }

        public bool IsSecondaryDocumentNeeded { get; set; }

        public LoanPurpose LoanPurpose { get; set; }
    }
}

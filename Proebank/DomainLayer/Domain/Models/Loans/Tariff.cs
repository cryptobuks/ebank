using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Domain.Enums;

namespace Domain.Models.Loans
{
    public class Tariff
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [DisplayName("Tariff Name")]
        public string Name { get; set; }

        [DisplayName("Int. Rate (0.3 means 30%)")]
        public decimal InterestRate { get; set; }

        [DisplayName("Min Amount")]
        public decimal MinAmount { get; set; }

        [DisplayName("Max Amount")]
        public decimal MaxAmount { get; set; }

        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayName("Start Date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Min Term")]
        public int MinTerm { get; set; }

        [DisplayName("Max Term")]
        public int MaxTerm { get; set; }

        [DisplayName("Min Age")]
        public ushort MinAge { get; set; }

        [DisplayName("Max Age")]
        public ushort? MaxAge { get; set; }

        [DisplayName("Initial Fee")]
        public decimal InitialFee { get; set; }

        [DisplayName("Guarantor")]
        public bool IsGuarantorNeeded { get; set; }

        [DisplayName("Secondary Doc.")]
        public bool IsSecondaryDocumentNeeded { get; set; }

        [DisplayName("Purpose")]
        public LoanPurpose LoanPurpose { get; set; }

        public bool Validate(LoanApplication loanApplication)
        {
            var amount = loanApplication.LoanAmount;
            var term = loanApplication.Term;
            var isAmountValid = amount >= MinAmount && amount <= MaxAmount;
            var isTermValid = term >= MinTerm && term <= MaxTerm;
            // TODO: complete
            return isAmountValid && isTermValid;
        }
    }
}

using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models.Loans
{
    public class Tariff : Entity
    {
        [DisplayName("Tariff Name")]
        public string Name { get; set; }
        
        [DisplayName("Interest Rate")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:P}")]
        public decimal InterestRate { get; set; }

        [DisplayName("Min Amount")]
        [Range(100, 100000000, ErrorMessage = "Amount value should be between 100 and 100000000")]
        [DataType(DataType.Currency)]
        public decimal MinAmount { get; set; }

        [DisplayName("Max Amount")]
        [Range(100, 100000000, ErrorMessage = "Amount value should be between 100 and 100000000")]
        [DataType(DataType.Currency)]
        public decimal MaxAmount { get; set; }

        [DisplayName("Creation Date")]
        public DateTime CreationDate { get; set; }

        [DisplayName("Active?")]
        public bool IsActive { get; set; }

        [DisplayName("Min Term")]
        [Range(1, 60, ErrorMessage = "Term value should be between 1 month and 5 years")]
        public int MinTerm { get; set; }

        [DisplayName("Max Term")]
        [Range(1, 60, ErrorMessage = "Term value should be between 1 month and 5 years")]
        public int MaxTerm { get; set; }

        [DisplayName("Payment frequency, in months")]
        [Range(0, 12)]
        public int PmtFrequency { get; set; }

        [DisplayName("Payment type")]
        public PaymentCalculationType PmtType { get; set; }

        [DisplayName("Min. allowed age")]
        [Range(18, 65, ErrorMessage = "Age value should be between 18 and 65 years")]
        public int MinAge { get; set; }

        [DisplayName("Max. allowed age")]
        [Range(18, 65, ErrorMessage = "Age value should be between 18 and 65 years")]
        public int MaxAge { get; set; }

        //[DisplayName("Initial Fee")]
        //public decimal InitialFee { get; set; }

        [DisplayName("Guarantor")]
        public bool IsGuarantorNeeded { get; set; }

        [DisplayName("Purpose")]
        public LoanPurpose LoanPurpose { get; set; }

        [DisplayName("Currency")]
        public Currency Currency { get; set; }
    }
}

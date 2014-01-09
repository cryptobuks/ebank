using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;
using Domain.Models.Loans;

namespace Presentation.Models
{
    public class TariffViewModel : EntityViewModel
    {
        [DisplayName("Tariff Name")]
        public string Name { get; set; }

        [DisplayName("Interest Rate")]
        [Range(0.1, 100000, ErrorMessage = "Interest value should be between 0.1% and 100000%")]
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

        [DisplayName("Is Active")]
        public bool IsActive { get; set; }

        [DisplayName("Minimal term")]
        [Range(1, 240, ErrorMessage = "Term value should be between 1 month and 20 years")]
        public int MinTerm { get; set; }

        [DisplayName("Maximal term")]
        [Range(1, 240, ErrorMessage = "Term value should be between 1 month and 20 years")]
        public int MaxTerm { get; set; }

        [DisplayName("Payment frequency, in months")]
        [Range(0, 12)]
        public int PmtFrequency { get; set; }

        [DisplayName("Payment type")]
        public PaymentCalculationType PmtType { get; set; }

        [DisplayName("Minimal allowed age")]
        [Range(18, 65, ErrorMessage = "Age value should be between 18 and 65 years")]
        public int MinAge { get; set; }

        [DisplayName("Maximal allowed age")]
        [Range(18, 65, ErrorMessage = "Age value should be between 18 and 65 years")]
        public int MaxAge { get; set; }

        [DisplayName("Guarantor")]
        public bool IsGuarantorNeeded { get; set; }

        [DisplayName("Purpose")]
        public LoanPurpose LoanPurpose { get; set; }

        [DisplayName("Currency")]
        public Currency Currency { get; set; }
        public TariffViewModel()
        {
            MinAge = 18;
            MaxAge = 65;
        }

        public TariffViewModel(Tariff tariff) : base(tariff)
        {
            Name = tariff.Name;
            InterestRate = tariff.InterestRate * 100;
            MinAmount = tariff.MinAmount;
            MaxAmount = tariff.MaxAmount;
            CreationDate = tariff.CreationDate;
            IsActive = tariff.IsActive;
            MinTerm = tariff.MinTerm;
            MaxTerm = tariff.MaxTerm;
            PmtFrequency = tariff.PmtFrequency;
            PmtType = tariff.PmtType;
            MinAge = tariff.MinAge;
            MaxAge = tariff.MaxAge;
            IsGuarantorNeeded = tariff.IsGuarantorNeeded;
            LoanPurpose = tariff.LoanPurpose;
            Currency = tariff.Currency;
        }

        public Tariff Convert()
        {
            // TODO: check Id
            var tariff = new Tariff
            {
                Id = Id,
                IsRemoved = IsRemoved,
                Name = Name,
                InterestRate = InterestRate / 100,
                MinAmount = MinAmount,
                MaxAmount = MaxAmount,
                CreationDate = CreationDate,
                IsActive = IsActive,
                MinTerm = MinTerm,
                MaxTerm = MaxTerm,
                PmtFrequency = PmtFrequency,
                PmtType = PmtType,
                MinAge = MinAge,
                MaxAge = MaxAge,
                IsGuarantorNeeded = IsGuarantorNeeded,
                LoanPurpose = LoanPurpose,
                Currency = Currency
            };
            return tariff;
        }
    }
}
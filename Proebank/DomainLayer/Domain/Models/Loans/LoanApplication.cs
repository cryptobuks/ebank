﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Domain.Enums;

namespace Domain.Models.Loans
{
    public class LoanApplication : Entity
    {
        [DisplayName("Loan amount")]
        [Range(100, 100000000)]
        public decimal LoanAmount { get; set; }

        [DisplayName("Time created")]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime TimeCreated { get; set; }

        [DisplayName("Time contracted")]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? TimeContracted { get; set; }

        [DisplayName("Term")]
        public int Term { get; set; }

        [DisplayName("Cell phone")]
        [Required]
        [Phone]
        [DataType(DataType.PhoneNumber)]
        public string CellPhone { get; set; }

        [DisplayName("E-mail")]
        [EmailAddress]
        public string Email { get; set; }

        public virtual Tariff Tariff { get; set; }

        public virtual Guid TariffId { get; set; }

        [DisplayName("Loan purpose")]
        public LoanPurpose LoanPurpose { get; set; }

        [DisplayName("Status")]
        public LoanApplicationStatus Status { get; set; }

        public virtual PersonalData PersonalData { get; set; }

        public virtual PersonalData Guarantor { get; set; }

        [DisplayName("Currency")]
        public Currency Currency { get; set; }

        [DisplayName("Middle income (for last 6 months)")]
        [Range(0.0, 10000000000)]
        public decimal MiddleIncome { get; set; }

        [DisplayName("Children count")]
        [Range(0, 50)]
        public int ChildrenCount { get; set; }

        [DisplayName("Education status")]
        public Education HigherEducation { get; set; }

        [DisplayName("Marital status")]
        public MaritalStatus IsMarried { get; set; }

        [DisplayName("Length of work")]
        [Range(0, 100)]
        public int LengthOfWork { get; set; }
        
        [DisplayName("Home owner?")]
        public bool IsHomeowner { get; set; }

        [DisplayName("Get money by card? ")]
        public bool ReceiveToCard { get; set; }
    }
}
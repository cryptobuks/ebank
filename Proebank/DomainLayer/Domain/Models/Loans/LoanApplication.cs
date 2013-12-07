using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Domain.Enums;

namespace Domain.Models.Loans
{
    public class LoanApplication : Entity
    {
        [DisplayName("Loan Amount")]
        [Range(0, 100000000)]
        public decimal LoanAmount { get; set; }

        [DisplayName("Time Created")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime TimeCreated { get; set; }

        [DisplayName("Time Contracted")]
        //[DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? TimeContracted { get; set; }

        [DisplayName("Term")]
        public int Term { get; set; }

        [DisplayName("Cell Phone")]
        public string CellPhone { get; set; }

        public virtual Tariff Tariff { get; set; }

        [DisplayName("Loan Purpose")]
        public LoanPurpose LoanPurpose { get; set; }

        [DisplayName("Status")]
        public LoanApplicationStatus Status { get; set; }

        [DisplayName("Documents")]
        public virtual ICollection<Document> Documents { get; set; }

        // TODO: implement explicit choice
        [DisplayName("Currency")]
        public Currency Currency { get; set; }
    }
}

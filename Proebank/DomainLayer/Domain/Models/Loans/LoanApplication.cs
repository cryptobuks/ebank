using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Domain.Enums;

namespace Domain.Models.Loans
{
    public class LoanApplication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public decimal LoanAmount { get; set; }

        public DateTime TimeCreated { get; set; }

        public int Term { get; set; }

        public string CellPhone { get; set; }

        public int TariffId { get; set; }

        public LoanPurpose LoanPurpose { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}

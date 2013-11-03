using System;
using System.Collections.Generic;
using Domain.Enums;

namespace Domain.Models
{
    public class LoanApplication
    {
        public long Id { get; set; }

        public decimal LoanAmount { get; set; }

        public DateTime TimeCreated { get; set; }

        public int Term { get; set; }

        public string CellPhone { get; set; }

        public Tariff Tariff { get; set; }

        public LoanPurpose LoanPurpose { get; set; }

        public virtual ICollection<Document> Documents { get; set; }
    }
}

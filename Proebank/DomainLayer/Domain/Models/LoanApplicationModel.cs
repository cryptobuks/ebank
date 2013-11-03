﻿using System;
using System.Collections.Generic;

namespace Domain.Models
{
    public class LoanApplicationModel
    {
        public long Id { get; set; }

        public decimal LoanAmount { get; set; }

        public DateTime TimeCreated { get; set; }

        public int Term { get; set; }

        public TariffModel Tariff { get; set; }

        public LoanPurpose LoanPurpose { get; set; }

        public IEnumerable<DocumentModel> Documents { get; set; }
    }
}

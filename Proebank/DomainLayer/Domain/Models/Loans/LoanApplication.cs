﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using Domain.Enums;

namespace Domain.Models.Loans
{
    public class LoanApplication
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public decimal LoanAmount { get; set; }

        public DateTime TimeCreated { get; set; }

        public DateTime? TimeContracted { get; set; }

        public int Term { get; set; }

        public string CellPhone { get; set; }

        public int TariffId { get; set; }

        public Tariff Tariff { get; set; }

        public LoanPurpose LoanPurpose { get; set; }

        public LoanApplicationStatus Status { get; private set; }

        public virtual ICollection<Document> Documents { get; set; }

        // TODO: implement explicit choice
        public Currency Currency
        {
            get { return Currency.BYR; }
        }

        public void Approve()
        {
            Status = LoanApplicationStatus.Approved;
        }

        public void Reject()
        {
            Status = LoanApplicationStatus.Rejected;
        }

        public void Contract()
        {
            TimeContracted = DateTime.Now;
            Status = LoanApplicationStatus.Contracted;
        }
    }
}
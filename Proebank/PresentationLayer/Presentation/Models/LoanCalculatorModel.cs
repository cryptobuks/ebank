using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Models.Loans;

namespace Presentation.Models
{
    public class LoanCalculatorModel
    {
        public Guid TariffId { get; set; }

        [Display(Name = "Loan Amount")]
        public decimal LoanAmount { get; set; }

        [Display(Name = "Term(in month)")]
        public int Term { get; set; }

        public IEnumerable<Payment> Payments { get; set; }
    }
}
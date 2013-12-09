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

        public IEnumerable<SelectListItem> Tariffs { get; set; }
        
        [Display(Name = "Sum")]
        public decimal Sum { get; set; }

        [Display(Name = "Term(in month)")]
        public int Term { get; set; }

        [Display(Name = "Paymnets")]
        public IEnumerable<Payment> Payments { get; set; }
    }
}
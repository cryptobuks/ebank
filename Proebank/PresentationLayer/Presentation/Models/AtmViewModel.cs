using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Presentation.Models
{
    public class AtmViewModel
    {
        [Display(Name = "Loan Id")]
        public Guid LoanId { get; set; }

        [Display(Name = "Amount")]
        public decimal Amount { get; set; }
    }
}
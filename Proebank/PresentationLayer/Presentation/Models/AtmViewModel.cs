using System;
using System.ComponentModel.DataAnnotations;

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
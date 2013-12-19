using System;
using System.ComponentModel.DataAnnotations;

namespace Presentation.Models
{
    public class AtmViewModel
    {
        [Display(Name = "Loan Id")]
        [Required]
        public Guid LoanId { get; set; }

        [Display(Name = "Amount")]
        [Required(ErrorMessage = "Amount is Required")]
        [Range(0, int.MaxValue, ErrorMessage = "Amount must be a positive number")]
        public decimal Amount { get; set; }
    }
}
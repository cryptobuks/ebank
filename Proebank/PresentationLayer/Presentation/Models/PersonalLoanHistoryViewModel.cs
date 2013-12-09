using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Domain.Models.Loans;

namespace Presentation.Models
{
    public class PersonalLoanHistoryViewModel
    {
        public string Id { get; set; }

        [Display(Name = "Application")]
        public LoanApplication Application { get; set; }

        [Display(Name = "History")]
        public List<LoanHistory> History { get; set; }
    }
}
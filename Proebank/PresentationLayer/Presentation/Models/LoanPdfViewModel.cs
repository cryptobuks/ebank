using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Models.Loans;

namespace Presentation.Models
{
    public class LoanPdfViewModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public Loan Loan { get; set; }
    }
}
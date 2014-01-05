using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Models.Loans;

namespace Presentation.Models
{
    public class PdfBill
    {
        public decimal Amount { get; set; }
        public Loan Loan { get; set; }
        public string Operator { get; set; }
    }
}
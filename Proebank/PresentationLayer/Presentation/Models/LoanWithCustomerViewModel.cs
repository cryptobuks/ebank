using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Models.Customers;
using Domain.Models.Loans;

namespace Presentation.Models
{
    public class LoanWithCustomerViewModel
    {
        public Loan Loan { get; set; }
        public Customer Customer { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models.Loans
{
    public class LoanHistory : Entity
    {
        public Document Person { get; set; }
        public DateTime WhenOpened { get; set; }
        public DateTime? WhenClosed { get; set; }
        public decimal Amount { get; set; }
        public bool HadProblems { get; set; }
    }
}

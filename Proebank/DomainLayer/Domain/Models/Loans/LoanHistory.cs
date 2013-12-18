using Domain.Enums;
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
        public virtual PersonalData Person { get; set; }
        public DateTime WhenOpened { get; set; }
        public DateTime? WhenClosed { get; set; }
        public decimal Amount { get; set; }
        public Currency Currency { get; set; }
        public bool HadProblems { get; set; }
    }
}

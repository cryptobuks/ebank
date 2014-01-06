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
        public Guid LoanId { get; set; }

        public LoanHistory()
        {
        }

        public LoanHistory(Loan loan)
        {
            if (loan != null)
            {
                var application = loan.Application;
                if (application != null && application.TimeContracted.HasValue)
                {
                    Person = application.PersonalData;
                    WhenOpened = application.TimeContracted.Value;
                    Amount = application.LoanAmount;
                    Currency = application.Currency;
                    LoanId = loan.Id;
                }
                else throw new ArgumentException("loan.Application");
            }
            else throw new ArgumentException("loan");
        }
    }
}

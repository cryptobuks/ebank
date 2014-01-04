using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;

namespace Domain.Models.Loans
{
    public class CommitteeVoting : Entity
    {
        public string EmployeeId { get; set; }
        public Guid LoanApplicationId { get; set; }
        public LoanApplicationCommitteeMemberStatus Status { get; set; }

        public CommitteeVoting()
        {
            
        }
        public CommitteeVoting(string employeeId, Guid loanApplicationId, LoanApplicationCommitteeMemberStatus status)
        {
            EmployeeId = employeeId;
            LoanApplicationId = loanApplicationId;
            Status = status;
        }
    }
}

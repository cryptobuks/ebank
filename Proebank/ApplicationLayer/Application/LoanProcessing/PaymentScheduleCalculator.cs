using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    class PaymentScheduleCalculator
    {
        internal static PaymentSchedule Calculate(LoanApplication loanApplication)
        {
            // TODO: make it more complex; check for validness
            var status = loanApplication.Status;
            DateTime? startDate;
            switch (status)
            {
                case LoanApplicationStatus.Contracted:
                    startDate = loanApplication.TimeContracted;
                    break;
                case LoanApplicationStatus.Approved:
                case LoanApplicationStatus.New:
                    startDate = loanApplication.TimeCreated;
                    break;
                default:
                    throw new ArgumentException("schedule should not be calculated for application of status=" + loanApplication.Status);
            }
            if (startDate == null)
            {
                throw new Exception("Loan application of its status must have its time");
            }
            // TODO: very, very poor logic. FIX IT!!!
            var validStartDate = startDate.Value.AddMonths(1).AddDays(28 - startDate.Value.Day);
            var finalAmount = loanApplication.LoanAmount*(1 + loanApplication.Tariff.InterestRate);
            var term = loanApplication.Term;
            var part = finalAmount / term;
            var schedule = new PaymentSchedule();
            for (var i = 1; i <= term; i++)
            {
                schedule.AddPayment(new Payment { Amount = part, ShouldBePaidBefore = validStartDate.AddMonths(i) });
            }
            return schedule;
        }
    }
}

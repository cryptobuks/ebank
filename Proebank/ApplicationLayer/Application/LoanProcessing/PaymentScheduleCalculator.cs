using System;
using System.Globalization;
using Domain.Enums;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    public class PaymentScheduleCalculator
    {
        internal static PaymentSchedule Calculate(LoanApplication loanApplication)
        {
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
                throw new Exception("Loan application of its status must have its time (created or contracted)");
            }

            var tariff = loanApplication.Tariff;
            var loanAmount = loanApplication.LoanAmount;
            var term = loanApplication.Term;
            var finalAmount = InterestCalculator.TotalSum(tariff, loanAmount, term);
            var part = finalAmount / term;
            var schedule = new PaymentSchedule();
            for (var i = 1; i <= term; i++)
            {
                schedule.AddPayment(new Payment { Amount = part, ShouldBePaidBefore = CalculatePaymentDate(startDate.Value, i) });
            }
            return schedule;
        }

        private static DateTime CalculatePaymentDate(DateTime startDate, int i)
        {
            var paymentDate = startDate.AddMonths(i);
            paymentDate = new DateTime(paymentDate.Year, paymentDate.Month, DateTime.DaysInMonth(paymentDate.Year, paymentDate.Month)).AddDays(1).AddTicks(-1);
            //while (paymentDate.DayOfWeek == DayOfWeek.Saturday || paymentDate.DayOfWeek == DayOfWeek.Sunday)
            //{
            //    paymentDate = paymentDate.AddDays(1);
            //}
            return paymentDate;
        }

        public static PaymentSchedule CalculatePaymentScheduleWithoutDateTime(decimal sum, Tariff tariff, int term)
        {
            if (term > tariff.MaxTerm || term < tariff.MinTerm)
            {
                throw new ArgumentException(String.Format("Term is not within the range of Tariff : {0}", tariff.Name));
            }
            if (sum > tariff.MaxAmount || sum < tariff.MinAmount)
            {
                throw new ArgumentException(String.Format("Sum is not within the range of Tariff : {0}",tariff.Name));
            }
            
            var totalSum = InterestCalculator.TotalSum(tariff, sum, term);
            var partSum = totalSum / term;

            var schedule = new PaymentSchedule();
            for (var i = 1; i <= term; i++)
            {
                schedule.AddPayment(new Payment { Amount = partSum});
            }
            return schedule;
        }
    }
}

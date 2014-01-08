using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Domain.Enums;
using Domain.Models.Loans;

namespace Application.LoanProcessing
{
    public class PaymentScheduleCalculator
    {
        public static PaymentSchedule Calculate(decimal loanAmount, Tariff tariff, int term, DateTime? startDate = null)
        {
            if (tariff == null)
            {
                throw new ArgumentNullException("tariff");
            }
            if (term > tariff.MaxTerm || term < tariff.MinTerm)
            {
                throw new ArgumentException(String.Format("Term is not within the range of Tariff : {0}", tariff.Name));
            }
            if (loanAmount > tariff.MaxAmount || loanAmount < tariff.MinAmount)
            {
                throw new ArgumentException(String.Format("Sum is not within the range of Tariff : {0}", tariff.Name));
            }

            switch (tariff.PmtType)
            {
                case PaymentCalculationType.Annuity:
                    return CalculateAnnuitySchedule(tariff, loanAmount, term, startDate);
                case PaymentCalculationType.Standard:
                    return CalculateStandardSchedule(tariff, loanAmount, term, startDate);
                default:
                    throw new ArgumentException("Unknown payment calculation type: " + tariff.PmtType);
            }
        }

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
                    throw new ArgumentException("schedule should not be calculated for application in status " + loanApplication.Status);
            }
            if (startDate == null)
            {
                throw new Exception("Loan application of its status must have its time (created or contracted)");
            }

            var tariff = loanApplication.Tariff;
            var loanAmount = loanApplication.LoanAmount;
            var term = loanApplication.Term;

            return Calculate(loanAmount, tariff, term, startDate);
        }

        public static PaymentSchedule CalculateAnnuitySchedule(Tariff tariff, decimal loanAmount, int term, DateTime? startDate)
        {
            var rate = tariff.InterestRate * tariff.PmtFrequency / 12;
            var helpCoeff = PowDecimal(1 + rate, term);
            var annuityCoeff = (rate * helpCoeff) / (helpCoeff - 1);
            var monthlyPayment = loanAmount * annuityCoeff;

            var schedule = new PaymentSchedule();
            var remainMainDebt = loanAmount;
            for (var i = 0; i < term; i++)
            {
                var pmtInterest = remainMainDebt * rate;
                var pmtMainDebt = monthlyPayment - pmtInterest;
                var accruedDate = CalculateAccrualDate(startDate, i + 1);
                var payBefore = CalculatePaymentDate(startDate, i + 1);
                if (startDate.HasValue && payBefore > startDate.Value.AddMonths(term))
                {
                    payBefore = startDate.Value.AddMonths(term);
                }
                if (i == 0 && startDate.HasValue)
                {
                    pmtInterest *= ((30 - startDate.Value.Day + 1) / 30M);
                }
                if (i + 1 == term && startDate.HasValue)
                {
                    pmtInterest += pmtInterest * ((startDate.Value.Day - 1) / 30M);
                }
                var pmt = new Payment
                {
                    MainDebtAmount = pmtMainDebt,
                    AccruedInterestAmount = pmtInterest,
                    AccruedOn = accruedDate,
                    ShouldBePaidBefore = payBefore
                };
                schedule.AddPayment(pmt);
                remainMainDebt -= pmtMainDebt;
            }
            return RoundPayments(schedule, 2);
        }

        private static PaymentSchedule CalculateStandardSchedule(Tariff tariff, decimal loanAmount, int term, DateTime? startDate)
        {
            var rate = tariff.InterestRate * tariff.PmtFrequency / 12;
            var pmtMainDebt = loanAmount / term;
            var schedule = new PaymentSchedule();
            var remainMainDebt = loanAmount;
            for (var i = 0; i < term; i++)
            {
                var pmtInterest = remainMainDebt * rate;
                var payBefore = CalculatePaymentDate(startDate, i + 1);
                if (i == 0 && startDate.HasValue)
                {
                    pmtInterest *= ((30 - startDate.Value.Day + 1) / 30M);
                }
                if (i + 1 == term && startDate.HasValue)
                {
                    pmtInterest += pmtInterest * ((startDate.Value.Day - 1) / 30M);
                }
                if (startDate.HasValue && payBefore > startDate.Value.AddMonths(term))
                {
                    payBefore = startDate.Value.AddMonths(term);
                }

                var pmt = new Payment
                {
                    MainDebtAmount = pmtMainDebt,
                    AccruedInterestAmount = pmtInterest,
                    AccruedOn = CalculateAccrualDate(startDate, i),
                    ShouldBePaidBefore = payBefore
                };
                schedule.AddPayment(pmt);
                remainMainDebt -= pmtMainDebt;
            }
            return RoundPayments(schedule, 2);
        }

        private static DateTime? CalculatePaymentDate(DateTime? startDate, int i)
        {
            if (startDate == null) return null;
            var date = startDate.Value;
            var pmtDate = date.AddMonths(i);
            // This is for getting the first day of the month
            pmtDate = new DateTime(pmtDate.Year, pmtDate.Month, DateTime.DaysInMonth(pmtDate.Year, pmtDate.Month)).AddDays(1).AddTicks(-1);
            if (pmtDate.DayOfWeek == DayOfWeek.Saturday || pmtDate.DayOfWeek == DayOfWeek.Sunday)
            {
                pmtDate = pmtDate.AddDays(pmtDate.DayOfWeek == DayOfWeek.Saturday ? 2 : 1);
            }
            return pmtDate;
        }

        private static DateTime? CalculateAccrualDate(DateTime? startDate, int i)
        {
            if (startDate == null) return null;
            var date = startDate.Value.Date;
            var accrualDate = date.AddMonths(i);
            // This is for getting the first day of the month
            accrualDate = new DateTime(accrualDate.Year, accrualDate.Month, 1);
            if (accrualDate.DayOfWeek == DayOfWeek.Saturday || accrualDate.DayOfWeek == DayOfWeek.Sunday)
            {
                accrualDate = accrualDate.AddDays(accrualDate.DayOfWeek == DayOfWeek.Saturday ? 2 : 1);
            }
            return accrualDate;
        }

        private static decimal PowDecimal(decimal x, int y)
        {
            if (y < 0) throw new ArgumentException("y");
            var result = 1M;
            for (var i = 0; i < y; i++)
            {
                result *= x;
            }
            return result;
        }

        private static PaymentSchedule RoundPayments(PaymentSchedule paymentSchedule, int digitsAfterZero)
        {
            var resultPaymentSchedule = new PaymentSchedule();
            var deltaMainDebt = 0M;
            var deltaInterest = 0M;
            var deltaOverdueMainDebt = 0M;
            var deltaOverdueInterest = 0M;
            if (paymentSchedule.Payments.Any())
            {
                var lastPmt = paymentSchedule.Payments.Last();
                foreach (var payment in paymentSchedule.Payments)
                {
                    if (payment == null) continue;

                    var newMainDebt = 0M;
                    var newInterest = 0M;
                    var newOverdueMainDebt = 0M;
                    var newOverdueInterest = 0M;
                    if (payment != lastPmt)
                    {
                        newMainDebt = Math.Round(payment.MainDebtAmount, digitsAfterZero);
                        newInterest = Math.Round(payment.AccruedInterestAmount, digitsAfterZero);
                        newOverdueMainDebt = Math.Round(payment.OverdueMainDebtAmount, digitsAfterZero);
                        newOverdueInterest = Math.Round(payment.OverdueInterestAmount, digitsAfterZero);

                        deltaMainDebt += payment.MainDebtAmount - newMainDebt;
                        deltaInterest += payment.AccruedInterestAmount - newInterest;
                        deltaOverdueMainDebt += payment.OverdueMainDebtAmount - newOverdueMainDebt;
                        deltaOverdueInterest += payment.OverdueInterestAmount - newOverdueInterest;
                    }
                    else
                    {
                        newMainDebt = Math.Round(payment.MainDebtAmount + deltaMainDebt, digitsAfterZero);
                        newInterest = Math.Round(payment.AccruedInterestAmount + deltaInterest, digitsAfterZero);
                        newOverdueMainDebt = Math.Round(payment.OverdueMainDebtAmount + deltaOverdueMainDebt, digitsAfterZero);
                        newOverdueInterest = Math.Round(payment.OverdueInterestAmount + deltaOverdueInterest, digitsAfterZero);
                    }

                    resultPaymentSchedule.AddPayment(new Payment
                        {
                            MainDebtAmount = newMainDebt,
                            AccruedInterestAmount = newInterest,
                            OverdueMainDebtAmount = newOverdueMainDebt,
                            OverdueInterestAmount = newOverdueInterest,
                            AccruedOn = payment.AccruedOn,
                            ShouldBePaidBefore = payment.ShouldBePaidBefore,
                        });
                }
            }
            return resultPaymentSchedule;
        }
    }
}

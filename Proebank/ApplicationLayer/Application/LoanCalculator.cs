using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.LoanProcessing;
using Domain.Models.Loans;

namespace Application
{
    public class LoanCalculator
    {
        private PaymentSchedule _paymentSchedule;

        public decimal Sum { get; set; }
        public Tariff Tariff { get; set; }
        public int Term { get; set; }

        public PaymentSchedule PaymentSchedule
        {
            get { return _paymentSchedule; }
        }

        public LoanCalculator(decimal sum, Tariff tariff, int term)
        {
            Sum = sum;
            Tariff = tariff;
            Term = term;
        }

        public void Calculate()
        {
            _paymentSchedule = PaymentScheduleCalculator.CalculatePaymentScheduleWithoutDateTime(Sum, Tariff, Term);
        }

        //Подсчитывает итоговую сумму
        //public decimal TotalSum() 


    }
}

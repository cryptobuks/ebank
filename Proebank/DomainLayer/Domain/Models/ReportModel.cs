using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class ReportModel
    {
        public ReportModel(DateTime dateFrom, DateTime dateTo)
        {
            DateFrom = dateFrom;
            DateTo = dateTo;
        }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int LoanApplicationsProcessed { get; set; }
        public int LoanApplicationsApproved { get; set; }
        public int LoansSigned { get; set; }
        public string MostPopularTariff { get; set; }
        public string LeastPopularTariff { get; set; }
        public decimal OverallLoanAmountIssuedByr { get; set; }
        public decimal OverallLoanAmountIssuedEur { get; set; }
        public decimal OverallLoanAmountIssuedUsd { get; set; }
        public decimal BankIncomeByr { get; set; }
        public decimal BankIncomeEur { get; set; }
        public decimal BankIncomeUsd { get; set; }
        public double LoanApplicationsApprovalPercentage
        {
            get
            {
                return LoanApplicationsProcessed != 0
                    ? Math.Round(100.0 * LoanApplicationsApproved / LoanApplicationsProcessed, 2)
                    : 0;
            }
        }
    }
}

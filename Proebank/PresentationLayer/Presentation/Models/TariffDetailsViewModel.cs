using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Presentation.Models
{
    public class TariffDetailsViewModel
    {
        public string Name { get; set; }

        [DisplayName("Applications created")]
        public int LoanApplicationsCreated { get; set; }

        [DisplayName("Applications approval percentage")]
        public double LoanApplicationApprovalPercentage { get; set; }

        [DisplayName("Loans issued")]
        public int LoansIssued { get; set; }

        [DisplayName("Loans in active status")]
        public int LoansActive { get; set; }
    }
}
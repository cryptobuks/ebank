using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain.Models.Loans;
using Domain;
using Application;

namespace Presentation.Controllers
{
    public class StatisticController : Controller
    {
        private ProcessingService _service = new ProcessingService();

        public ActionResult LoanApplication()
        {
            var list = _service.GetLoanApplications().ToList();
            return View(list);
        }

        public ActionResult Loan()
        {
            var list = _service.GetLoans().ToList();
            return View(list);
        }
    }
}

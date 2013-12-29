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
using Microsoft.Practices.Unity;

namespace Presentation.Controllers
{
    public class StatisticController : BaseController
    {
        [Dependency]
        protected ProcessingService Service { get; set; }

        public ActionResult LoanApplication()
        {
            var list = Service.GetLoanApplications().ToList();
            return View(list);
        }

        public ActionResult Loan()
        {
            var list = Service.GetLoans().ToList();
            return View(list);
        }
    }
}

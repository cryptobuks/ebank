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

        public ActionResult Index()
        {
            var list = _service.GetLoanApplications();
            return View(list);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application;
using Application.LoanProcessing;
using Domain.Repositories;
using Microsoft.Practices.Unity;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class AtmController : BaseController
    {
        private LoanRepository LoanService { get; set; }
        private ProcessingService ProcessingService { get; set; }

        public AtmController()
        {
            LoanService = Container.Resolve<LoanRepository>();
            ProcessingService = Container.Resolve<ProcessingService>();
        }
        public ActionResult Index()
        {
            var loans = LoanService.GetLoans(l => true);
            ViewBag.LoanId = new SelectList(loans, "Id", "Id");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AtmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loan = LoanService.GetLoans(l => l.Id == model.LoanId).FirstOrDefault();
                if (loan != null)
                {
                    ProcessingService.RegisterPayment(loan, model.Amount);
                }
            }
            ViewBag.PaymentRegistered = true;
            return RedirectToAction("Index");
        }
    }
}
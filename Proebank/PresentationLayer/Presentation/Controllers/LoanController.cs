using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application.LoanProcessing;
using Microsoft.Practices.Unity;
using Domain.Models.Loans;
using Application;
using System.Net;

namespace Presentation.Controllers
{
    public class LoanController : BaseController
    {
        private LoanService LoanService { get; set; }
        private ProcessingService _processingService { get; set; }

        public LoanController()
        {
            // TODO: remove something or create loan service property
            LoanService = Container.Resolve<LoanService>();
            _processingService = Container.Resolve<ProcessingService>();
        }

        public ActionResult Index()
        {
            var loans = LoanService.GetAll();
            return View(loans.ToList());    // TODO: why ToList() is needed?
        }

        public ActionResult Preview(LoanApplication loanApplication)
        {
            if (loanApplication == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(loanApplication);
        }

        public ActionResult Sign(LoanApplication loanApplication)
        {
            if (loanApplication == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loan = _processingService.CreateLoanContract(loanApplication);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", loan);
        }
    }
}
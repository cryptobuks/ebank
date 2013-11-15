using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application.LoanProcessing;
using Domain.Enums;
using Microsoft.Practices.Unity;
using Domain.Models.Loans;
using Application;
using System.Net;

namespace Presentation.Controllers
{
    public class LoanController : BaseController
    {
        private ProcessingService _processingService { get; set; }

        public LoanController()
        {
            // TODO: remove something or create loan service property
            _processingService = Container.Resolve<ProcessingService>();
        }

        public ActionResult Index()
        {
            var loans = _processingService.LoanService.GetLoans(la => true);
            return View(loans);
        }

        public ActionResult Preview(Guid? loanApplicationId)
        {
            if (loanApplicationId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _processingService.LoanService.GetLoanApplications(la => la.Id.Equals(loanApplicationId.Value)).SingleOrDefault();
            if (loanApplication == null || loanApplication.Status != LoanApplicationStatus.Approved)
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
            loanApplication = _processingService.LoanService.GetLoanApplications(la => la.Id.Equals(loanApplication.Id)).Single();
            var loan = _processingService.CreateLoanContract(loanApplication);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index", loan);
        }
    }
}
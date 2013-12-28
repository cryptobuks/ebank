using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Application;
using Domain.Enums;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using Presentation.Models;
using Domain;

namespace Presentation.Controllers
{
    public class CommitteeController : BaseController
    {
        [Dependency]
        protected ProcessingService Service { get; set; }

        // GET: /Committee/
        [Authorize(Roles = "Credit committee")]
        public ActionResult Index()
        {
            var loanapplications = Service.GetLoanApplications()
                .Where(la => la.Status == LoanApplicationStatus.UnderCommitteeConsideration && !la.IsRemoved);
            return View(loanapplications);
        }

        // POST: /Security/Approve/5
        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Credit committee")]
        public ActionResult Approved(Guid id)
        {
            
            var loanapplication = Service.Find<LoanApplication>(id);
            Service.ApproveLoanAppication(loanapplication);
            return RedirectToAction("Index");
        }

        // POST: /Security/Reject/5
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Credit committee")]
        public ActionResult Rejected(Guid id)
        {
            var loanapplication = Service.GetLoanApplications().SingleOrDefault(la => la.Id == id);
            Service.RejectLoanApplication(loanapplication);
            return RedirectToAction("Index");
        }
    }
}

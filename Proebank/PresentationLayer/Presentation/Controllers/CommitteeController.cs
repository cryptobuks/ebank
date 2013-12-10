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
using Presentation.Models;
using Domain;

namespace Presentation.Controllers
{
    public class CommitteeController : BaseController
    {
        private readonly ProcessingService _service = new ProcessingService();

        // GET: /Committee/
        [Authorize(Roles = "Credit committee")]
        public ActionResult Index()
        {
            var loanapplications = _service.GetLoanApplications(la =>
                la.Status == LoanApplicationStatus.UnderCommitteeConsideration && !la.IsRemoved);
            return View(loanapplications.ToList());
        }

        // POST: /Security/Approve/5
        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Credit committee")]
        public ActionResult Approved(Guid id)
        {
            var loanapplication = _service.GetLoanApplications(la => la.Id == id).SingleOrDefault();
            _service.ApproveLoanAppication(loanapplication);
            return RedirectToAction("Index");
        }

        // POST: /Security/Reject/5
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Credit committee")]
        public ActionResult Rejected(Guid id)
        {
            var loanapplication = _service.GetLoanApplications(la => la.Id == id).SingleOrDefault();
            _service.RejectLoanApplication(loanapplication);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

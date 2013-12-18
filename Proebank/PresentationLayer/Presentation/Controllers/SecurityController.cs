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
using Domain;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class SecurityController : BaseController
    {
        private readonly ProcessingService _service = new ProcessingService();

        // GET: /Security/
        [Authorize(Roles = "Security")]
        public ActionResult Index()
        {
            var loanapplications = _service.GetLoanApplications()
                .Where(la => la.Status == LoanApplicationStatus.UnderRiskConsideration && !la.IsRemoved);
            return View(loanapplications);
        }

        // GET: /Security/Details/5
        [Authorize(Roles = "Security")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var application = _service.GetLoanApplications().SingleOrDefault(la => la.Id == id);
            if (application == null)
            {
                return HttpNotFound();
            }
            var history = _service.GetHistoryFromNationalBank(application);
            if (history == null)
            {
                return HttpNotFound();
            }
            var customerId = application.PersonalData.Customer.Id;
            var viewModel = new PersonalLoanHistoryViewModel { Id = customerId, Application = application, History = history.ToList() };
            return View(viewModel);
        }

        // POST: /Security/Approve/5
        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Security")]
        public ActionResult Approved(Guid id)
        {
            var loanapplication = _service.GetLoanApplications().SingleOrDefault(la => la.Id == id);
            _service.ApproveLoanAppication(loanapplication);
            return RedirectToAction("Index");
        }

        // POST: /Security/Reject/5
        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Security")]
        public ActionResult Rejected(Guid id)
        {
            var loanapplication = _service.GetLoanApplications().SingleOrDefault(la => la.Id == id);
            _service.RejectLoanApplication(loanapplication);
            return RedirectToAction("Index");
        }

        // POST: /Security/SendToCommittee/5
        [HttpPost, ActionName("SendToCommittee")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Security")]
        public ActionResult SendToCommittee(Guid id)
        {
            var loanapplication = _service.GetLoanApplications().SingleOrDefault(la => la.Id == id);
            _service.SendLoanApplicationToCommittee(loanapplication);
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

using System.Net;
using System.Web.Routing;
using Domain.Enums;
using Domain.Models.Loans;
using Domain.Repositories;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
using System.Web.Mvc;
using Application.LoanProcessing;
using System.Collections.Generic;
using Application;

namespace Presentation.Controllers
{
    public class LoanApplicationController : BaseController
    {
        private ProcessingService _service { get; set; }

        public LoanApplicationController()
        {
            _service = new ProcessingService();
        }

        public ActionResult Index()
        {
            var loanApplications = _service.GetLoanApplications(la => true);//.Include(l => l.Tariff);
            ViewBag.ActiveTab = "All";
            // TODO: get rid of ToList()
            return View(loanApplications);
        }


        public ActionResult New()
        {
            var loanApplications = _service
                .GetLoanApplications(a => a.Status == LoanApplicationStatus.New);
            ViewBag.ActiveTab = "New";
            return View("Index", loanApplications);
        }


        public ActionResult Approved()
        {
            var loanApplications = _service
                .GetLoanApplications(a => a.Status == LoanApplicationStatus.Approved);
            ViewBag.ActiveTab = "Approved";
            return View("Index", loanApplications);
        }


        public ActionResult Rejected()
        {
            var loanApplications = _service.GetLoanApplications(a => a.Status == LoanApplicationStatus.Rejected);
            ViewBag.ActiveTab = "Rejected";
            return View("Index", loanApplications);
        }


        public ActionResult Contracted()
        {
            var loanApplications = _service.GetLoanApplications(a => a.Status == LoanApplicationStatus.Contracted);
            ViewBag.ActiveTab = "Contracted";
            return View("Index", loanApplications);
        }


        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return View(loanApplication);
        }

        public ActionResult Create()
        {
            var tariffs = _service.GetTariffs(t => true);
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoanApplication loanApplication)
        {
            loanApplication.Status = LoanApplicationStatus.New;
            loanApplication.TimeCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
                loanApplication.Id = Guid.NewGuid();
                loanApplication.Documents = new List<Document>();
                loanApplication.TimeCreated = DateTime.UtcNow;
                loanApplication.Status = LoanApplicationStatus.New;
                loanApplication.Tariff = _service.GetTariffs(t => t.Id.Equals(loanApplication.Tariff.Id)).Single();
                _service.CreateLoanApplication(loanApplication);
                return RedirectToAction("Index");
            }

            // TODO: change dropdown list to fill application tariff at once
            var tariffs = _service.GetTariffs(t => true);
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", loanApplication.Tariff.Id);
            return View(loanApplication);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanapplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            var tariffs = _service.GetTariffs(t => true);
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", loanapplication.Tariff.Id);
            return View(loanapplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanApplication loanApplication)
        {
            if (ModelState.IsValid)
            {
                _service.UpsertLoanApplication(loanApplication);
                return RedirectToAction("Index");
            }
            var tariffs = _service.GetTariffs(t => true);
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", loanApplication.Tariff.Id);
            return View(loanApplication);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return View(loanApplication);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            _service.DeleteLoanApplicationById(id);
            return RedirectToAction("Index");
        }


        public ActionResult Contract(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            // TODO: try save changes if everything else doesn't work
            return RedirectToAction("Preview", "Loan", new { loanApplicationId = loanApplication.Id});
        }

        public ActionResult Approve(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            _service.ApproveLoanAppication(loanApplication);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(Guid id)
        {
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication != null)
            {
                _service.ApproveLoanAppication(loanApplication);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Reject(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            _service.RejectLoanApplication(loanApplication);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(Guid id)
        {
            LoanApplication loanApplication = _service.GetLoanApplications(l => l.Id.Equals(id)).Single();
            if (loanApplication != null)
            {
                _service.RejectLoanApplication(loanApplication);
            }
            return RedirectToAction("Index");
        }
    }
}
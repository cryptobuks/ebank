using System.Net;
using Domain.Enums;
using Domain.Models.Loans;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Application;

namespace Presentation.Controllers
{
    public class LoanApplicationController : BaseController
    {
        private readonly ProcessingService _service;

        public LoanApplicationController()
        {
            _service = new ProcessingService();
        }

        //[Authorize(Roles = "Department head, Consultant, Security service")]
        public ActionResult Index()
        {
            var loanApplications = _service.GetLoanApplications(la => true);//.Include(l => l.Tariff);
            ViewBag.ActiveTab = "All";
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
            var tariffs = _service.GetTariffs();
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
                _service.CreateLoanApplication(loanApplication);
                return RedirectToAction("Index");
            }

            var tariffs = _service.GetTariffs();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
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
            var tariffs = _service.GetTariffs();
            ViewBag.Tariff = new SelectList(tariffs, "Id", "Name");
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
            var tariffs = _service.GetTariffs();
            ViewBag.Tariff = new SelectList(tariffs, "Id", "Name");
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
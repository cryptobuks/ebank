using System.Collections.ObjectModel;
using System.Net;
using System.Web.Configuration;
using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Loans;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Collections.Generic;
using Application;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class LoanApplicationController : BaseController
    {
        private readonly ProcessingService _service;

        public LoanApplicationController()
        {
            _service = new ProcessingService();
        }

        [Authorize(Roles = "Department head, Consultant, Security, Credit committee")]
        public ActionResult Index()
        {
            if (User.IsInRole("Consultant"))
            {
                return RedirectToAction("New");
            }
            if (User.IsInRole("Security"))
            {
                return RedirectToAction("OnSecurityReview");
            }
            if (User.IsInRole("Credit committee"))
            {
                return RedirectToAction("OnCommitteeReview");
            }
            if (User.IsInRole("Department head"))
            {
                ViewBag.ActiveTab = "All";
                var loanApplications = _service.GetLoanApplications(true).ToList();
                return View(loanApplications);
            }
            return new HttpUnauthorizedResult();
        }

        public ActionResult New()
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.New)
                .ToList();
            ViewBag.ActiveTab = "New";
            return View("Index", loanApplications);
        }

        public ActionResult PreApproved()
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Filled)
                .ToList();
            ViewBag.ActiveTab = "PreApproved";
            return View("Index", loanApplications);
        }

        public ActionResult Reviewed()
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Approved || a.Status == LoanApplicationStatus.Rejected)
                .ToList();
            ViewBag.ActiveTab = "Reviewed";
            return View("Index", loanApplications);
        }

        public ActionResult OnSecurityReview()
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.UnderRiskConsideration)
                .ToList();
            ViewBag.ActiveTab = "Security";
            return View("Index", loanApplications);
        }

        public ActionResult OnCommitteeReview()
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.UnderCommitteeConsideration)
                .ToList();
            ViewBag.ActiveTab = "Committee";
            return View("Index", loanApplications);
        }

        public ActionResult Contracted()
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Contracted)
                .ToList();
            ViewBag.ActiveTab = "Contracted";
            return View("Index", loanApplications);
        }

        public ActionResult Approved()
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Approved)
                .ToList();
            ViewBag.ActiveTab = "Approved";
            return View("Index", loanApplications);
        }

        public ActionResult Rejected()
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Rejected)
                .ToList();
            ViewBag.ActiveTab = "Rejected";
            return View("Index", loanApplications);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications()
                .Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return View(loanApplication);
        }

        [AllowAnonymous]
        public ActionResult Create(Guid? id)
        {
            if (User.IsInRole("Consultant"))
            {
                return RedirectToAction("Fill");
            }


            var tariffs = _service.GetTariffs();
            ViewBag.Tariffs = new SelectList(tariffs, "Id", "Name", tariffs.FirstOrDefault(t => t.Id == id));

            if (TempData["loanApplication"] != null)
            {
                LoanApplication loanApplication = (LoanApplication)TempData["loanApplication"];
                return View(loanApplication);
            }
            
            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Create(LoanApplication loanApplication, string btnUseLoanCalculator)
        {
            if (btnUseLoanCalculator != null && btnUseLoanCalculator == "Use Loan Calculator")
            {
                    if (ModelState.IsValid)
                    {
                        TempData.Add("loanApplication", loanApplication);
                        return RedirectToAction("Index", "LoanCalculator");
                    }
            }
            else
            {
            loanApplication.Status = LoanApplicationStatus.New;
            loanApplication.TimeCreated = DateTime.Now;
            if (ModelState.IsValid)
            {
                try
                {
                    _service.CreateLoanApplication(loanApplication);
                }
                catch (ArgumentException e)
                {
                    var validationResult = e.Data["validationResult"] as Dictionary<string, string>;
                    if (validationResult != null)
                    {
                        foreach (var result in validationResult)
                        {
                            ModelState.AddModelError(result.Key, result.Value);
                        }
                        var tariffList = _service.GetTariffs();
                            ViewBag.Tariffs = new SelectList(tariffList, "Id", "Name");
                        return View();
                    }
                }
                if (!User.Identity.IsAuthenticated || User.IsInRole("Customer"))
                {
                    return View("Created");
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            }

            var tariffs = _service.GetTariffs();
            ViewBag.Tariffs = new SelectList(tariffs, "Id", "Name");
            return View(loanApplication);
        }


        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanapplication = _service.GetLoanApplications().Single(l => l.Id == id);
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            var tariffs = _service.GetTariffs().ToList();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
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
            var tariffs = _service.GetTariffs().ToList();
            ViewBag.Tariff = new SelectList(tariffs, "Id", "Name");
            return View(loanApplication);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
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
            var loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Preview", "Loan", new { loanApplicationId = loanApplication.Id });
        }

        public ActionResult Approve(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
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
            var loanApplication = _service.Find<LoanApplication>(id);
            if (loanApplication != null)
            {
                _service.ApproveLoanAppication(loanApplication);
            }
            return RedirectToAction("Index");
        }

        //[HttpPost, ActionName("ThumbsUp")]
        //[ValidateAntiForgeryToken]
        //public ActionResult ThumbsUp(Guid id)
        //{
        //    var loanApplication = _service.Find<LoanApplication>(id);
        //    if (loanApplication != null)
        //    {
        //        _service.ThumbsUpLoanAppication(loanApplication);
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost, ActionName("ThumbsDown")]
        //[ValidateAntiForgeryToken]
        //public ActionResult ThumbsDown(Guid id)
        //{
        //    var loanApplication = _service.Find<LoanApplication>(id);
        //    if (loanApplication != null)
        //    {
        //        _service.ThumbsDownLoanAppication(loanApplication);
        //    }
        //    return RedirectToAction("Index");
        //}

        public ActionResult Reject(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
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
            LoanApplication loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication != null)
            {
                _service.RejectLoanApplication(loanApplication);
            }
            return RedirectToAction("Index");
        }

        public ActionResult History(Guid id)
        {
            //var la = _service.FindLoanApplication(id);
            var la = _service.Find<LoanApplication>(id);
            if (la == null)
            {
                return HttpNotFound("There is no loan application with provided id");
            }
            var history = _service.GetHistoryFromNationalBank(la);
            return View(history);
        }

        public ActionResult SendToSecurity(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            _service.SendLoanApplicationToSecurity(loanApplication);
            return RedirectToAction("Index");
        }

        public ActionResult SendToCommittee(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            _service.SendLoanApplicationToCommittee(loanApplication);
            return RedirectToAction("Index");
        }


        public ActionResult Fill(Guid? id)
        {
            var tariffs = _service.GetTariffs();
            if (id == null)
            {
                //if filling base data by Consultant
                ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
                return View(new LoanApplication());
            }
            var loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }

            var selectedTariffId = loanApplication.TariffId.ToString();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", selectedTariffId);
            return View(loanApplication);
        }

        [HttpPost, ActionName("Fill")]
        [ValidateAntiForgeryToken]
        public ActionResult Fill(LoanApplication loanApplication)
        {
            if (ModelState.IsValid)
            {

                // connect to db to refresh connection
                // saving of loanApplication doesn't save docs
                var applicationWithDbRef = _service.GetLoanApplications().FirstOrDefault(l => l.Id.Equals(loanApplication.Id));
                if (applicationWithDbRef != null)
                {
                applicationWithDbRef.Status = LoanApplicationStatus.Filled;
                applicationWithDbRef.PersonalData = loanApplication.PersonalData;
                _service.UpsertLoanApplication(applicationWithDbRef);
            }
                else
                {
                    loanApplication.Status = LoanApplicationStatus.Filled;
                    _service.CreateLoanApplication(loanApplication, true);
                }
            }
            var tariffList = _service.GetTariffs();
            ViewBag.TariffId = new SelectList(tariffList, "Id", "Name");
            return RedirectToAction("Index");
        }
    }
}
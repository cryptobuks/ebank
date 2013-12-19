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
using PagedList;
using PagedList.Mvc;  

namespace Presentation.Controllers
{
    public class LoanApplicationController : BaseController
    {
        private readonly ProcessingService _service;
        private const int PAGE_SIZE = 5;

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
                return RedirectToAction("Security");
            }
            if (User.IsInRole("Credit committee"))
            {
                return RedirectToAction("Committee");
            }
            if (User.IsInRole("Department head"))
            {
                return RedirectToAction("All");
            }
            return new HttpUnauthorizedResult();
        }

        [Authorize(Roles = "Department head")]
        public ActionResult All(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications(true)
                .ToList();
            ViewBag.ActiveTab = "All";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult New(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.New)
                .ToList();
            ViewBag.ActiveTab = "New";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult PreApproved(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Filled)
                .ToList();
            ViewBag.ActiveTab = "PreApproved";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult Reviewed(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Approved || a.Status == LoanApplicationStatus.Rejected)
                .ToList();
            ViewBag.ActiveTab = "Reviewed";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult Security(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.UnderRiskConsideration)
                .ToList();
            ViewBag.ActiveTab = "Security";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult Committee(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.UnderCommitteeConsideration)
                .ToList();
            ViewBag.ActiveTab = "Committee";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult Contracted(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Contracted)
                .ToList();
            ViewBag.ActiveTab = "Contracted";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult Approved(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Approved)
                .ToList();
            ViewBag.ActiveTab = "Approved";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult Rejected(int? page)
        {
            var loanApplications = _service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Rejected)
                .ToList();
            ViewBag.ActiveTab = "Rejected";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
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

            LoanApplication loanApplication;
            if (id == null)
            {
                //if filling base data by Consultant
                loanApplication = new LoanApplication();
                if (TempData["loanApplication"] != null) //if response from Calculator
                {
                    loanApplication = (LoanApplication) TempData["loanApplication"];
                }
                ViewBag.Tariff = new SelectList(tariffs, "Id", "Name");
                return View(loanApplication);
            }
            loanApplication = _service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }

            var selectedTariffId = loanApplication.TariffId.ToString();
            ViewBag.Tariff = new SelectList(tariffs, "Id", "Name", selectedTariffId);
            return View(loanApplication);
        }

        [HttpPost, ActionName("Fill")]
        [ValidateAntiForgeryToken]
        public ActionResult Fill(LoanApplication loanApplication)
        {
            var tariffList = _service.GetTariffs();
            ViewBag.Tariff = new SelectList(tariffList, "Id", "Name");

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
                    try
                    {
                        _service.CreateLoanApplication(loanApplication, true);
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
                            return View(loanApplication);
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }
    }
}
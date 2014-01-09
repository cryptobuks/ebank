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
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Presentation.Models;
using PagedList;
using PagedList.Mvc;  

namespace Presentation.Controllers
{
    public class LoanApplicationController : BaseController
    {
        [Dependency]
        protected ProcessingService Service { get; set; }
        private const int PAGE_SIZE = 10;
        //SearchBy
        private const string SEARCHBY_TARIFF = "Tariff Name";
        private const string SEARCHBY_CELLPHONE = "Cell Phone";
        private const string SEARCHBY_IDENTIFICATIONNUMBER = "Identification Number";
        private const string SEARCHBY_PASSPORT_NUMBER = "Passport number";
        private const string SEARCHBY_FIRST_NAME = "First Name";
        private const string SEARCHBY_LAST_NAME = "Last Name";
        private const string SEARCHBY_STATUS = "Status";
        //SortBy
        private const string SORTBY_TARIFF_ASC = "Tariff ASC";
        private const string SORTBY_TARIFF_DESC = "Tariff DESC";

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

        [Authorize(Roles = "Credit committee")]
        public ActionResult ApproveCommittee(Guid id)
        {
            Service.AddCommitteeVoting(User.Identity.GetUserId(), id, LoanApplicationCommitteeMemberStatus.Approved);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Credit committee")]
        public ActionResult RejectCommittee(Guid id)
        {
            Service.AddCommitteeVoting(User.Identity.GetUserId(), id, LoanApplicationCommitteeMemberStatus.Rejected);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Department head")]
        public ActionResult All(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service
                .GetLoanApplications(true)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER},
                    new SelectListItem() {Text = SEARCHBY_STATUS, Value = SEARCHBY_STATUS}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "All";
            ViewBag.AllCommitteeVotings = Service.GetCommitteeVotings().ToList();
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Consultant, Department head")]
        public ActionResult New(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.New)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "New";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Consultant, Department head")]
        public ActionResult PreApproved(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Filled)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "PreApproved";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Consultant, Department head")]
        public ActionResult Reviewed(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service.GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Approved || a.Status == LoanApplicationStatus.Rejected)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "Reviewed";
            return View("Index", loanApplications.ToList().ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Security, Department head")]
        public ActionResult Security(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.UnderRiskConsideration)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "Security";
            ViewBag.Scoring = loanApplications.ToList()
                .ToDictionary(la => la.Id,
                la => ScoringSystem.CalculateRating(la, Service.GetHistoryFromNationalBank(la)));
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Credit committee, Department head")]
        public ActionResult Committee(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.UnderCommitteeConsideration)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;


            ViewBag.ActiveTab = "Committee";
            List<CommitteeVoting> cv = Service.GetCommitteeVotings().Where(x => x.EmployeeId == User.Identity.GetUserId()).ToList();
            ViewBag.CommiteeVotings = cv;
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Contracted(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Contracted)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "Contracted";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Consultant, Department head")]
        public ActionResult Approved(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Approved)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "Approved";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Consultant, Department head")]
        public ActionResult Rejected(int? page, string searchBy, string search, string sortBy)
        {
            var loanApplications = Service
                .GetLoanApplications()
                .Where(a => a.Status == LoanApplicationStatus.Rejected)
                .AsQueryable();

            loanApplications = Searching(searchBy, search, loanApplications);
            loanApplications = Sorting(sortBy, loanApplications);

            //for sortBy
            ViewBag.NextSortTariffParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;

            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "Rejected";
            return View("Index", loanApplications.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = Service.GetLoanApplications()
                .Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return View();
            }
            return View(loanApplication);
        }

        [AllowAnonymous]
        public ActionResult Create(Guid? id)
        {
            if (User.IsInRole("Consultant"))
            {
                return RedirectToAction("Fill", new {tariffId = id});
            }

            var tariffs = Service.GetTariffs().Where(t => t.IsActive);
            ViewBag.Tariffs = new SelectList(tariffs, "Id", "Name", id);

            if (TempData["loanApplication"] != null)
            {
                var loanApplication = (LoanApplication)TempData["loanApplication"];
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
                        Service.CreateLoanApplication(loanApplication);
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
                            var tariffList = Service.GetTariffs().Where(t => t.IsActive);
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

            var tariffs = Service.GetTariffs().Where(t => t.IsActive);
            ViewBag.Tariffs = new SelectList(tariffs, "Id", "Name");
            return View(loanApplication);
        }


        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var loanapplication = Service.Find<LoanApplication>(id);
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            var tariffs = Service.GetTariffs().Where(t => t.IsActive).ToList();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
            return View(loanapplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanApplication loanApplication)
        {
            if (ModelState.IsValid)
            {
                Service.UpsertLoanApplication(loanApplication);
                return RedirectToAction("Index");
            }
            var tariffs = Service.GetTariffs().Where(t => t.IsActive).ToList();
            ViewBag.Tariff = new SelectList(tariffs, "Id", "Name");
            return View(loanApplication);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var loanApplication = Service.GetLoanApplications().Single(l => l.Id == id);
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
            Service.DeleteLoanApplicationById(id);
            return RedirectToAction("Index");
        }

        public ActionResult Contract(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = Service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Preview", "Loan", new { loanApplicationId = loanApplication.Id });
        }

        public ActionResult ApproveCommite(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Approve(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = Service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            Service.ApproveLoanAppication(loanApplication);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(Guid id)
        {
            var loanApplication = Service.Find<LoanApplication>(id);
            if (loanApplication != null)
            {
                Service.ApproveLoanAppication(loanApplication);
            }
            return RedirectToAction("Index");
        }

        //[HttpPost, ActionName("ThumbsUp")]
        //[ValidateAntiForgeryToken]
        //public ActionResult ThumbsUp(Guid id)
        //{
        //    var loanApplication = Service.Find<LoanApplication>(id);
        //    if (loanApplication != null)
        //    {
        //        Service.ThumbsUpLoanAppication(loanApplication);
        //    }
        //    return RedirectToAction("Index");
        //}

        //[HttpPost, ActionName("ThumbsDown")]
        //[ValidateAntiForgeryToken]
        //public ActionResult ThumbsDown(Guid id)
        //{
        //    var loanApplication = Service.Find<LoanApplication>(id);
        //    if (loanApplication != null)
        //    {
        //        Service.ThumbsDownLoanAppication(loanApplication);
        //    }
        //    return RedirectToAction("Index");
        //}

        public ActionResult Reject(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = Service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            Service.RejectLoanApplication(loanApplication);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(Guid id)
        {
            LoanApplication loanApplication = Service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication != null)
            {
                Service.RejectLoanApplication(loanApplication);
            }
            return RedirectToAction("Index");
        }

        public ActionResult History(Guid id)
        {
            //var la = Service.FindLoanApplication(id);
            var la = Service.Find<LoanApplication>(id);
            if (la == null)
            {
                return HttpNotFound("There is no loan application with provided id");
            }
            var history = Service.GetHistoryFromNationalBank(la);
            return View(history);
        }

        public ActionResult SendToSecurity(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = Service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            Service.SendLoanApplicationToSecurity(loanApplication);
            return RedirectToAction("Index");
        }

        public ActionResult SendToCommittee(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = Service.GetLoanApplications().Single(l => l.Id == id);
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            Service.SendLoanApplicationToCommittee(loanApplication);
            return RedirectToAction("Index");
        }


        public ActionResult Fill(Guid? id, Guid? tariffId)
        {
            var tariffs = Service.GetTariffs().Where(t => t.IsActive).ToList();
            var tariffGuarantor = tariffs.Select(t => new {Id = t.Id, isGuarantorNeeded = t.IsGuarantorNeeded});
            ViewBag.tariffGuarantor = tariffGuarantor;
            LoanApplication loanApplication;
            if (id == null)
            {
                //if filling base data by Consultant
                loanApplication = new LoanApplication();
                if (TempData["loanApplication"] != null) //if response from Calculator
                {
                    loanApplication = (LoanApplication)TempData["loanApplication"];
                }
                ViewBag.Tariff = new SelectList(tariffs, "Id", "Name", tariffId);
                return View(loanApplication);
            }
            loanApplication = Service.GetLoanApplications().Single(l => l.Id == id);
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
            var tariffList = Service.GetTariffs();
            ViewBag.Tariff = new SelectList(tariffList, "Id", "Name");

            //if (ModelState.IsValid)
            //{
                // connect to db to refresh connection
                // saving of loanApplication doesn't save docs
                var applicationWithDbRef = Service.GetLoanApplications().FirstOrDefault(l => l.Id.Equals(loanApplication.Id));
                if (applicationWithDbRef != null)
                {
                    applicationWithDbRef.Status = LoanApplicationStatus.Filled;
                    applicationWithDbRef.PersonalData = loanApplication.PersonalData;
                    Service.UpsertLoanApplication(applicationWithDbRef);
                }
                else
                {
                    loanApplication.Status = LoanApplicationStatus.Filled;
                    try
                    {
                        Service.CreateLoanApplication(loanApplication, true);
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
            //}
            return RedirectToAction("Index");
        }


        private static IQueryable<LoanApplication> Searching(string searchBy, string search, IQueryable<LoanApplication> loanApplications)
        {
            var loanApplicationsResults = loanApplications;

            switch (searchBy)
            {
                case SEARCHBY_TARIFF:
                    loanApplicationsResults = loanApplicationsResults.Where(la => la.Tariff.Name.Contains(search) || search == null);
                    break;
                case SEARCHBY_CELLPHONE:
                    loanApplicationsResults = loanApplicationsResults.Where(la => la.CellPhone.Contains(search) || search == null);
                    break;
                case SEARCHBY_IDENTIFICATIONNUMBER:
                    loanApplicationsResults =
                        loanApplicationsResults.Where(la => la.PersonalData.Identification.Contains(search) || search == null);
                    break;
                case SEARCHBY_FIRST_NAME:
                    loanApplicationsResults = loanApplicationsResults.Where(la => la.PersonalData.FirstName.Contains(search) || search == null);
                    break;
                case SEARCHBY_LAST_NAME:
                    loanApplicationsResults = loanApplicationsResults.Where(la => la.PersonalData.LastName.Contains(search) || search == null);
                    break;
                case SEARCHBY_PASSPORT_NUMBER:
                    loanApplicationsResults = loanApplicationsResults.Where(la => la.PersonalData.Passport.Contains(search) || search == null);
                    break;
                case SEARCHBY_STATUS:
                    loanApplicationsResults =
                        loanApplicationsResults.ToList()
                        .Where(la => la.Status.ToString().Contains(search) || search == null)
                        .AsQueryable();
                    break;
            }
            return loanApplicationsResults;
        }


        private static IQueryable<LoanApplication> Sorting(string sortBy, IQueryable<LoanApplication> loanApplications)
        {
            var loanApplicationsResults = loanApplications;

            switch (sortBy)
            {
                case SORTBY_TARIFF_ASC:
                    loanApplicationsResults = loanApplicationsResults.OrderBy(la => la.Tariff.Name);
                    break;
                case SORTBY_TARIFF_DESC:
                    loanApplicationsResults = loanApplicationsResults.OrderByDescending(la => la.Tariff.Name);
                    break;
                default: //if sortBy == empty = OrderByDefault => orderBy Tarif ASC
                    loanApplicationsResults = loanApplicationsResults.OrderBy(la => la.Tariff.Name);
                    break;
            }
            return loanApplicationsResults;
        }
    }
}
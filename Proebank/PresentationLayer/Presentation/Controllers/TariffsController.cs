using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Application;
using Microsoft.Practices.Unity;
using PagedList;
using PagedList.Mvc;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class TariffsController : BaseController
    {
        //Amount of elements to display on one page of PagedList
        private const int PAGE_SIZE = 10;

        [Dependency]
        protected ProcessingService Service { get; set; }

        [AllowAnonymous]
        public ActionResult Index(int? page)
        {
            var isHead = User.IsInRole("Department head");
            var tariffs = Service.GetTariffs().Where(t => isHead || t.IsActive).ToList();
            ViewBag.ActiveTab = "Index";
            return View(tariffs.ToPagedList(page ?? 1,PAGE_SIZE));
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            
            var tariff = Service.Find<Tariff>(id);
            if (tariff == null)
            {
                return RedirectToAction("Index");
            }
            var tariffDetails = new TariffDetailsViewModel();
            var loans = Service.GetLoans();
            var loanApps = Service.GetLoanApplications();
            tariffDetails.Name = tariff.Name;
            tariffDetails.LoanApplicationsCreated = loanApps.Count(la => la.TariffId == id);
            tariffDetails.LoanApplicationApprovalPercentage = tariffDetails.LoanApplicationsCreated != 0
                ? Math.Round(
                    loanApps.Count(
                        la =>
                            la.Status != LoanApplicationStatus.New && la.Status != LoanApplicationStatus.Filled &&
                            la.Status != LoanApplicationStatus.Rejected)*100.0/tariffDetails.LoanApplicationsCreated, 2)
                : 0;
            tariffDetails.LoansIssued = loans.Count(l => l.Application.TariffId == id);
            tariffDetails.LoansActive = loans.Count(l => !l.IsClosed && l.Application.TariffId == id);
            return View(tariffDetails);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Tariffs/Create
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [Authorize(Roles = "Department head")]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TariffViewModel tariff)
        {
            tariff.PmtFrequency = 1;
            if (tariff.MinAge > tariff.MaxAge)
            {
                ModelState.AddModelError("MinAge", "Minimal age is greater than maximal");
            }
            if (tariff.MinAmount > tariff.MaxAmount)
            {
                ModelState.AddModelError("MinAmount", "Minimal loan amount is greater than maximal");
            }
            if (tariff.MinTerm > tariff.MaxTerm)
            {
                ModelState.AddModelError("MinTerm", "Minimal term is greater than maximal");
            }
            if (Service.GetTariffs().Any(t => t.Name.ToLower() == tariff.Name.ToLower()))
            {
                ModelState.AddModelError("Name", "There is already tariff with equal name");
            }
            if (ModelState.IsValid)
            {
                tariff.CreationDate = Service.GetCurrentDate();
                Service.UpsertTariff(tariff.Convert());
                return RedirectToAction("Index");
            }

            return View(tariff);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            
            var tariff = Service.Find<Tariff>(id);
            if (tariff == null)
            {
                return RedirectToAction("Index");
            }
            ViewBag.CanBeEdited = !Service.GetLoans().Any(l => l.Application.TariffId == id) &&
                !Service.GetLoanApplications().Any(la => la.TariffId == id);
            return View(new TariffViewModel(tariff));
        }

        // POST: /Tariffs/Edit/5
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [Authorize(Roles = "Department head")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TariffViewModel tariff)
        {
            tariff.PmtFrequency = 1;
            if (tariff.MinAge > tariff.MaxAge)
            {
                ModelState.AddModelError("MinAge", "Minimal age is greater than maximal");
            }
            if (tariff.MinAmount > tariff.MaxAmount)
            {
                ModelState.AddModelError("MinAmount", "Minimal loan amount is greater than maximal");
            }
            if (tariff.MinTerm > tariff.MaxTerm)
            {
                ModelState.AddModelError("MinTerm", "Minimal term is greater than maximal");
            }
            if (ModelState.IsValid)
            {
                Service.UpsertTariff(tariff.Convert());
                return RedirectToAction("Index");
            }
            var id = tariff.Id;
            ViewBag.CanBeEdited = !Service.GetLoans().Any(l => l.Application.TariffId == id) &&
                !Service.GetLoanApplications().Any(la => la.TariffId == id);
            return View(tariff);
        }

        // GET: /Tariffs/Delete/5
        [Authorize(Roles = "Department head")]
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            var tariff = Service.Find<Tariff>(id);
            if (tariff == null)
            {
                return HttpNotFound();
            }
            tariff.IsActive = false;
            Service.UpsertTariff(tariff);
            return RedirectToAction("Index");
        }

        //// POST: /Tariffs/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[Authorize(Roles = "Department head")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    Service.DeleteTariffById(id);
        //    return RedirectToAction("Index");
        //}

        [Authorize(Roles = "Department head")]
        public ActionResult Activate(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tariff = Service.Find<Tariff>(id);
            if (tariff == null)
            {
                return HttpNotFound();
            }
            tariff.IsActive = true;
            Service.UpsertTariff(tariff);
            return RedirectToAction("Index");
        }
    }
}

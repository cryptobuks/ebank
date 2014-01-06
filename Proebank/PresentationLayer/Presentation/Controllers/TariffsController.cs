using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Application;
using Microsoft.Practices.Unity;
using PagedList;
using PagedList.Mvc;

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

        [AllowAnonymous]
        public ActionResult Details(Guid? id)
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
            return View(tariff);
        }

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
        public ActionResult Create(Tariff tariff)
        {
            if (ModelState.IsValid)
            {
                
                tariff.Id = Guid.NewGuid();
                tariff.CreationDate = Service.GetCurrentDate();
                Service.UpsertTariff(tariff);
                return RedirectToAction("Index");
            }

            return View(tariff);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Edit(Guid? id)
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
            ViewBag.CanBeEdited = !Service.GetLoans().Any(l => l.Application.TariffId == id) &&
                !Service.GetLoanApplications().Any(la => la.TariffId == id);
            return View(tariff);
        }

        // POST: /Tariffs/Edit/5
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [Authorize(Roles = "Department head")]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tariff tariff)
        {
            tariff.PmtFrequency = 1;
            if (ModelState.IsValid)
            {
                Service.UpsertTariff(tariff);
                return RedirectToAction("Index");
            }
            return View(tariff);
        }

        // GET: /Tariffs/Delete/5
        [Authorize(Roles = "Department head")]
        public ActionResult Delete(Guid? id)
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

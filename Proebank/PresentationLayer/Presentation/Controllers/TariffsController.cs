using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain.Models.Loans;
using Infrastructure;
using Infrastructure.Repositories;
using Microsoft.Practices.Unity;

namespace Presentation.Controllers
{
    public class TariffsController : BaseController
    {
        private ITariffRepository TariffRepository { get; set; }

        public TariffsController()
        {
            TariffRepository = Container.Resolve<ITariffRepository>();
        }

        public ActionResult Index()
        {
            var tariffs = TariffRepository.GetAll().ToList();
            return View(tariffs);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tariff tariff = TariffRepository.Get(t => t.Id.Equals(id));
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
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tariff tariff)
        {
            if (ModelState.IsValid)
            {
                tariff.Id = Guid.NewGuid();
                TariffRepository.SaveOrUpdate(tariff);
                return RedirectToAction("Index");
            }

            return View(tariff);
        }

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tariff tariff = TariffRepository.Get(t => t.Id.Equals(id));
            if (tariff == null)
            {
                return HttpNotFound();
            }
            return View(tariff);
        }

        // POST: /Tariffs/Edit/5
		// To protect from over posting attacks, please enable the specific properties you want to bind to, for 
		// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
		// 
		// Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Tariff tariff)
        {
            if (ModelState.IsValid)
            {
                TariffRepository.SaveOrUpdate(tariff);
                return RedirectToAction("Index");
            }
            return View(tariff);
        }

        // GET: /Tariffs/Delete/5
        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tariff tariff = TariffRepository.Get(t => t.Id.Equals(id));
            if (tariff == null)
            {
                return HttpNotFound();
            }
            return View(tariff);
        }

        // POST: /Tariffs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            Tariff tariff = TariffRepository.Get(t => t.Id.Equals(id));
            TariffRepository.Delete(tariff);
            return RedirectToAction("Index");
        }
    }
}

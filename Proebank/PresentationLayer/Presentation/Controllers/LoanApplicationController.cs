using System.Data.Entity;
using System.Net;
using Application;
using Domain.Models.Accounts;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class LoanApplicationController : BaseController
    {
        private ILoanApplicationRepository LoanApplicationRepository { get; set; }
        private ITariffRepository TariffRepository { get; set; }

        public LoanApplicationController()
        {
            LoanApplicationRepository = Container.Resolve<ILoanApplicationRepository>();
            TariffRepository = Container.Resolve<ITariffRepository>();
        }

        public ActionResult Index()
        {
            var loanapplications = LoanApplicationRepository.GetAll();//.Include(l => l.Tariff);
            return View(loanapplications.ToList());
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanapplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            return View(loanapplication);
        }

        public ActionResult Create()
        {
            var tariffs = TariffRepository.GetAll();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
            return View();
        }

        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoanApplication loanapplication)
        {
            if (ModelState.IsValid)
            {
                loanapplication.Id = Guid.NewGuid();
                LoanApplicationRepository.SaveOrUpdate(loanapplication);
                //db.SaveChanges();
                return RedirectToAction("Index");
            }

            var tariffs = TariffRepository.GetAll();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", loanapplication.TariffId);
            return View(loanapplication);
        }

        // GET: /L/Edit/5
        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanapplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            var tariffs = TariffRepository.GetAll();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", loanapplication.TariffId);
            return View(loanapplication);
        }

        // To protect from over posting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        // 
        // Example: public ActionResult Update([Bind(Include="ExampleProperty1,ExampleProperty2")] Model model)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LoanApplication loanapplication)
        {
            if (ModelState.IsValid)
            {
                LoanApplicationRepository.SaveOrUpdate(loanapplication);
                return RedirectToAction("Index");
            }
            var tariffs = TariffRepository.GetAll();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", loanapplication.TariffId);
            return View(loanapplication);
        }

        public ActionResult Delete(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanapplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            return View(loanapplication);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(Guid id)
        {
            LoanApplication loanapplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            LoanApplicationRepository.Delete(loanapplication);
            return RedirectToAction("Index");
        }

        public ActionResult Approve(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            LoanApplicationRepository.Approve(loanApplication);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Approve")]
        [ValidateAntiForgeryToken]
        public ActionResult Approve(Guid id)
        {
            LoanApplication loanApplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanApplication != null)
            {
                LoanApplicationRepository.Approve(loanApplication);
            }
            return RedirectToAction("Index");
        }

        public ActionResult Reject(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LoanApplication loanApplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            LoanApplicationRepository.Reject(loanApplication);
            return RedirectToAction("Index");
        }

        [HttpPost, ActionName("Reject")]
        [ValidateAntiForgeryToken]
        public ActionResult Reject(Guid id)
        {
            LoanApplication loanApplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanApplication != null)
            {
                LoanApplicationRepository.Reject(loanApplication);
            }
            return RedirectToAction("Index");
        }
    }
}
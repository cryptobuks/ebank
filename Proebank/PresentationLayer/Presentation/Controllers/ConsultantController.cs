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

namespace Presentation.Controllers
{
    public class ConsultantController : BaseController
    {
        private readonly ProcessingService _service = new ProcessingService();

        // GET: /Consultant/
        public ActionResult Index()
        {
            var loanapplications = _service.GetLoanApplications(la => la.Status == LoanApplicationStatus.New);
            return View(loanapplications.ToList());
        }

        //// GET: /Consultant/Details/5
        //public ActionResult Details(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LoanApplication loanapplication = db.LoanApplications.Find(id);
        //    if (loanapplication == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(loanapplication);
        //}

        // GET: /Consultant/Create
        public ActionResult Create()
        {
            ViewBag.TariffId = new SelectList(_service.GetTariffs(), "Id", "Name");
            return View();
        }

        // POST: /Consultant/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(
                Include =
                    "Id,LoanAmount,TimeCreated,TimeContracted,Term,CellPhone,TariffId,LoanPurpose,Status,Currency,IsRemoved"
                )] LoanApplication loanapplication)
        {
            if (ModelState.IsValid)
            {
                loanapplication.Id = Guid.NewGuid();
                _service.CreateLoanApplication(loanapplication);
                return RedirectToAction("SecondStage", loanapplication);
            }
            return View(loanapplication);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Consultant")]
        public ActionResult SecondStage(LoanApplication application)
        {
            return View(application);
        }

        public ActionResult SendToSecurity(LoanApplication application)
        {
            _service.SendLoanApplicationToCommittee(application);
            return RedirectToAction("Index");
        }

        public ActionResult SendToCommittee(LoanApplication application)
        {
            _service.SendLoanApplicationToCommittee(application);
            return RedirectToAction("Index");
        }

        //    ViewBag.TariffId = new SelectList(db.Tariffs, "Id", "Name", loanapplication.TariffId);
        //    return View(loanapplication);
        //}

        //// GET: /Consultant/Edit/5
        //public ActionResult Edit(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LoanApplication loanapplication = db.LoanApplications.Find(id);
        //    if (loanapplication == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    ViewBag.TariffId = new SelectList(db.Tariffs, "Id", "Name", loanapplication.TariffId);
        //    return View(loanapplication);
        //}

        //// POST: /Consultant/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include="Id,LoanAmount,TimeCreated,TimeContracted,Term,CellPhone,TariffId,LoanPurpose,Status,Currency,IsRemoved")] LoanApplication loanapplication)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(loanapplication).State = EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    ViewBag.TariffId = new SelectList(db.Tariffs, "Id", "Name", loanapplication.TariffId);
        //    return View(loanapplication);
        //}

        //// GET: /Consultant/Delete/5
        //public ActionResult Delete(Guid? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    LoanApplication loanapplication = db.LoanApplications.Find(id);
        //    if (loanapplication == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(loanapplication);
        //}

        //// POST: /Consultant/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(Guid id)
        //{
        //    LoanApplication loanapplication = db.LoanApplications.Find(id);
        //    db.LoanApplications.Remove(loanapplication);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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

using System.Net;
using Domain.Enums;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using System;
using System.Linq;
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
            ViewBag.ActiveTab = "All";
            return View(loanapplications.ToList());
        }


        public ActionResult New()
        {
            var loanapplications = LoanApplicationRepository
                .GetAll(a => a.Status == LoanApplicationStatus.New);
            ViewBag.ActiveTab = "New";
            return View("Index", loanapplications.ToList());
        }


        public ActionResult Approved()
        {
            var loanapplications = LoanApplicationRepository
                .GetAll(a => a.Status == LoanApplicationStatus.Approved);
            ViewBag.ActiveTab = "Approved";
            return View("Index", loanapplications.ToList());
        }


        public ActionResult Rejected()
        {
            var loanapplications = LoanApplicationRepository
                .GetAll(a => a.Status == LoanApplicationStatus.Rejected);
            ViewBag.ActiveTab = "Rejected";
            return View("Index", loanapplications.ToList());
        }


        public ActionResult Contracted()
        {
            var loanapplications = LoanApplicationRepository
                .GetAll(a => a.Status == LoanApplicationStatus.Contracted);
            ViewBag.ActiveTab = "Contracted";
            return View("Index", loanapplications.ToList());
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LoanApplication loanapplication)
        {
            loanapplication.Status = LoanApplicationStatus.New;
            loanapplication.TimeCreated = DateTime.Now;
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

        public ActionResult Edit(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanapplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanapplication == null)
            {
                return HttpNotFound();
            }
            var tariffs = TariffRepository.GetAll();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name", loanapplication.TariffId);
            return View(loanapplication);
        }

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
            var loanApplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
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
            LoanApplication loanapplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            LoanApplicationRepository.Delete(loanapplication);
            return RedirectToAction("Index");
        }


        public ActionResult Contract(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = LoanApplicationRepository.Get(l => l.Id.Equals(id));
            if (loanApplication == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Preview", "Loan", loanApplication);
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
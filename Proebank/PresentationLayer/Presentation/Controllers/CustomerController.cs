using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain.Models.Loans;
using Domain;

namespace Presentation.Controllers
{
    public class CustomerController : Controller
    {
        private DataContext db = new DataContext();

        // GET: /Customer/
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            return View(db.Loans.ToList());
        }

        // GET: /Customer/Details/5
        [Authorize(Roles = "Customer")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound();
            }
            return View(loan);
        }

        // GET: /Customer/Details/5
        [Authorize(Roles = "Customer")]
        public ActionResult Schedule(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loan = db.Loans.Find(id);
            if (loan == null)
            {
                return HttpNotFound("Loan was not found");
            }
            var schedule = loan.PaymentSchedule;
            if (schedule == null)
            {
                return HttpNotFound("Schedule not found for loan");
            }
            return View(schedule.Payments);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

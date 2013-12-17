using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Application;

namespace Presentation.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ProcessingService _service = new ProcessingService();

        // GET: /Customer/
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            return View(_service.GetLoans());
        }

        // GET: /Customer/Details/5
        [Authorize(Roles = "Customer")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loan = _service.FindLoan(id);
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
            var loan = _service.FindLoan(id);
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
                _service.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

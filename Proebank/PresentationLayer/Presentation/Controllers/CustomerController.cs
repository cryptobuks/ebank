using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Application;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;

namespace Presentation.Controllers
{
    public class CustomerController : BaseController
    {
        [Dependency]
        protected ProcessingService Service { get; set; }

        // GET: /Customer/
        [Authorize(Roles = "Customer")]
        public ActionResult Index()
        {
            return View(Service.GetLoans());
        }

        // GET: /Customer/Details/5
        [Authorize(Roles = "Customer")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            var loan = Service.Find<Loan>(id);
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
            
            var loan = Service.Find<Loan>(id);
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
    }
}

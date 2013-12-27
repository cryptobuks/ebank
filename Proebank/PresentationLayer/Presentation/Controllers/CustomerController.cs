using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Application;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Microsoft.AspNet.Identity;
using Microsoft.Practices.Unity;
using Presentation.Models;

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
            var userId = User.Identity.GetUserId();
            var model = Service.GetLoans().Where(l => l.CustomerId == userId).ToList();
            return View(model);
        }

        // GET: /Customer/Details/5
        [Authorize(Roles = "Customer")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }
            
            var loan = Service.Find<Loan>(id);
            if (loan == null || loan.CustomerId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            var viewModel = new LoanDetailsViewModel(loan)
            {
                Customer = UnitOfWork.Context.Set<Customer>().Find(loan.CustomerId)
            };
            return View(viewModel);
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
            if (loan == null || loan.CustomerId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
                //return HttpNotFound("Loan was not found");
            }
            var schedule = loan.PaymentSchedule;
            if (schedule == null)
            {
                return RedirectToAction("Index");
                //return HttpNotFound("Schedule not found for loan");
            }
            return View(schedule.Payments);
        }
    }
}

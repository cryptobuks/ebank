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
using PagedList;
using PagedList.Mvc;

namespace Presentation.Controllers
{
    public class CustomerController : BaseController
    {
        //Amount of elements to display on one page of PagedList
        private const int PAGE_SIZE = 5;

        [Dependency]
        protected ProcessingService Service { get; set; }

        // GET: /Customer/
        [Authorize(Roles = "Customer, Department head")]
        public ActionResult Index(string customerId, string firstName, string lastName, int? page)
        {
            var userId = string.Empty;
            if (User.IsInRole("Customer"))
            {
                userId = User.Identity.GetUserId();
            } else if (User.IsInRole("Department head"))
            {
                userId = (customerId?? string.Empty);
            }
            ViewBag.UserFirstName = (firstName ?? "None");
            ViewBag.UserLastName = (lastName ?? "None");
            ViewBag.Action = "Index";
            //customerLoans может быть равен 0 !!!!
            var customerLoans = Service.GetLoans().Where(l => l.CustomerId == userId).ToList();
            if (!customerLoans.Any())
            {
                //TODO: добавить страницу с ошибкой!!!
            }
            
            return View(customerLoans.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        // GET: /Customer/Details/5
        [Authorize(Roles = "Customer, Department head")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index");
            }

            var loan = Service.Find<Loan>(id);
            if (loan == null)
            {
                if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("Index");
                }
                if (User.IsInRole("Department head"))
                {
                    //!!here is better Redirect to INDEX(with all neaded parametrs)
                    return RedirectToAction("All");
                }
                //Старую проверка ещё вписал -> if (loan == null || loan.CustomerId != User.Identity.GetUserId())
            }
            else if (User.IsInRole("Customer") && loan.CustomerId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }
            else
            {
                var viewModel = new LoanDetailsViewModel(loan)
                {
                    Customer = UnitOfWork.Context.Set<Customer>().Find(loan.CustomerId)
                };
                return View(viewModel);
            }

            return null;
        }

        // GET: /Customer/Details/5
        [Authorize(Roles = "Customer,Department head")]
        public ActionResult Schedule(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var loan = Service.Find<Loan>(id);
            if (loan == null)
            {
                if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("Index");
                }
                if (User.IsInRole("Department head"))
                {
                    //!!here is better Redirect to INDEX(with all neaded parametrs)
                    return RedirectToAction("All");
                }
                //Старую проверка ещё вписал -> if (loan == null || loan.CustomerId != User.Identity.GetUserId())
            }
            else if (User.IsInRole("Customer") && loan.CustomerId != User.Identity.GetUserId())
            {
                return RedirectToAction("Index");
            }

            var schedule = loan.PaymentSchedule;
            if (schedule == null)
            {
                if (User.IsInRole("Customer"))
                {
                    return RedirectToAction("Index");
                    //return HttpNotFound("Schedule not found for loan");
                }
                else if (User.IsInRole("Department head"))
                {
                    //!!here is better Redirect to INDEX(with all neaded parametrs)
                    return RedirectToAction("All");
                }
            }
            else
            {
                if (User.IsInRole("Department head"))
                {
                    var customer = Context.Set<Customer>().ToList().FirstOrDefault(c => c.Id == loan.CustomerId);
                    if (customer != null)
                    {
                        ViewBag.PersonalData = customer.PersonalData;
                    }
                }

                return View(schedule.Payments.OrderBy(p => p.ShouldBePaidBefore));
            }

            return null;
        }

        [Authorize(Roles = "Department head")]
        public ActionResult All(int? page)
        {
            var customers = Context.Set<Customer>().ToList();
            ViewBag.ActiveTab = "All";
            return View(customers.ToPagedList(page ?? 1, PAGE_SIZE));
        }
    }
}

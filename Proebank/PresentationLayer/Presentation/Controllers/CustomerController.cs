using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Application;
using Application.LoanProcessing;
using Domain.Models.Accounts;
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
        //SearchBy
        private const string SEARCHBY_TARIFF = "Tariff Name";
        private const string SEARCHBY_CELLPHONE = "Cell Phone";
        private const string SEARCHBY_IDENTIFICATIONNUMBER = "Identification Number";
        private const string SEARCHBY_PASSPORT_NUMBER = "Passport number";
        private const string SEARCHBY_FIRST_NAME = "First Name";
        private const string SEARCHBY_LAST_NAME = "Last Name";
        private const string SEARCHBY_STATUS = "Status";
        //SortBy
        private const string SORTBY_LASTNAME_ASC = "LastName ASC";
        private const string SORTBY_LASTNAME_DESC = "LastName DESC";
        private const string SORTBY_FIRSTNAME_ASC = "FirstName ASC";
        private const string SORTBY_FIRSTNAME_DESC = "FirstName DESC";
        [Dependency]
        protected ProcessingService Service { get; set; }

        // GET: /Customer/
        [Authorize(Roles = "Consultant, Customer, Department head")]
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
        [Authorize(Roles = "Consultant, Customer, Department head")]
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
                ViewBag.AdditionalSum = InterestCalculator.CalculateInterestForCustomerInformation(loan, Service.GetCurrentDate());
                return View(viewModel);
            }

            return null;
        }


        [Authorize(Roles = "Consultant, Customer, Department head")]
        public ActionResult PersonalInfo(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var strId = id.ToString();
            var customer = Context.Set<Customer>().SingleOrDefault(c => c.Id == strId);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [Authorize(Roles = "Consultant, Customer, Department head")]
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

        [Authorize(Roles = "Consultant, Department head")]
        public ActionResult All(int? page, string searchBy, string search, string sortBy)
        {
            var customers = Context.Set<Customer>().AsQueryable();

            customers = Searching(searchBy, search, customers);
            customers = Sorting(sortBy, customers);


            ViewBag.NextSortLastNameParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_LASTNAME_ASC)) ? SORTBY_LASTNAME_DESC : SORTBY_LASTNAME_ASC;
            ViewBag.NextSortFirstNameParameter = (sortBy != null && sortBy.Equals(SORTBY_FIRSTNAME_ASC)) ? SORTBY_FIRSTNAME_DESC : SORTBY_FIRSTNAME_ASC;

            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "All";
            return View(customers.ToPagedList(page ?? 1, PAGE_SIZE));
        }




        private static IQueryable<Customer> Searching(string searchBy, string search, IQueryable<Customer> customers)
        {
            var customersResults = customers;

            switch (searchBy)
            {
                case SEARCHBY_CELLPHONE:
                    customersResults = customersResults.Where(c => c.Phone.Contains(search) || search == null);
                    break;
                case SEARCHBY_IDENTIFICATIONNUMBER:
                    customersResults =
                        customersResults.Where(c => c.PersonalData.Identification.Contains(search) || search == null);
                    break;
                case SEARCHBY_FIRST_NAME:
                    customersResults = customersResults.Where(c => c.PersonalData.FirstName.Contains(search) || search == null);
                    break;
                case SEARCHBY_LAST_NAME:
                    customersResults = customersResults.Where(c => c.PersonalData.LastName.Contains(search) || search == null);
                    break;
                case SEARCHBY_PASSPORT_NUMBER:
                    customersResults = customersResults.Where(c => c.PersonalData.Passport.Contains(search) || search == null);
                    break;
            }
            return customersResults;
        }

        private static IQueryable<Customer> Sorting(string sortBy, IQueryable<Customer> customers)
        {
            var customersResults = customers;

            switch (sortBy)
            {
                case SORTBY_LASTNAME_ASC:
                    customersResults =
                        customersResults.OrderBy(c => c.PersonalData.LastName);
                    break;
                case SORTBY_LASTNAME_DESC:
                    customersResults =
                        customersResults.OrderByDescending(c => c.PersonalData.LastName);
                    break;
                case SORTBY_FIRSTNAME_ASC:
                    customersResults =
                        customersResults.OrderBy(c => c.PersonalData.FirstName);
                    break;
                case SORTBY_FIRSTNAME_DESC:
                    customersResults =
                        customersResults.OrderByDescending(c => c.PersonalData.FirstName);
                    break;
                default: //if sortBy == empty = OrderByDefault => orderBy Tarif ASC
                    customersResults = customersResults.OrderBy(c => c.PersonalData.LastName);
                    break;
            }
            return customersResults;
        }
    }
}

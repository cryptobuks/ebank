using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Application;
using System.Net;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using Presentation.Extensions;
using Presentation.Models;
using RazorPDF;
using PagedList;
using PagedList.Mvc;
using System.Collections.Generic;

namespace Presentation.Controllers
{
    public class LoanController : BaseController
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
        private const string SORTBY_TARIFF_ASC = "Tariff ASC";
        private const string SORTBY_TARIFF_DESC = "Tariff DESC";
        private const string SORTBY_ISCLOSED_ASC = "IsClosed ASC";
        private const string SORTBY_ISCLOSED_DESC = "IsClosed DESC";
        private const string SORTBY_CUSTOMER_ASC = "Customer ASC";
        private const string SORTBY_CUSTOMER_DESC = "Customer DESC";

        [Dependency]
        protected ProcessingService Service { get; set; }

        [Authorize(Roles = "Department head, Consultant, Security")]
        public ActionResult Index(int? page)
        {
            if (User.IsInRole("Consultant"))
            {
                return RedirectToAction("Active", page);
            }
            if (User.IsInRole("Security"))
            {
                return RedirectToAction("InTrouble", page);
            }
            if (User.IsInRole("Department head"))
            {
                return RedirectToAction("All", page);
            }
            return new HttpUnauthorizedResult();
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult Preview(Guid? loanApplicationId)
        {
            if (loanApplicationId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = Service.GetLoanApplications().SingleOrDefault(la => la.Id.Equals(loanApplicationId.Value));
            if (loanApplication == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (loanApplication.Status != LoanApplicationStatus.Approved && loanApplication.Status != LoanApplicationStatus.ContractPrinted)
            {
                return RedirectToAction("Index", "LoanApplication");
            }
            return View(loanApplication);
        }

        [HttpPost]
        [Authorize(Roles = "Department head, Consultant")]
        [ValidateAntiForgeryToken]
        public ActionResult Preview(LoanApplication loanApplication)
        {
            if (loanApplication == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var laId = loanApplication.Id;
            String password = null;
            // check customer here because of using default UserStore and UserManager
            var loanApplicationSet = UnitOfWork.GetDbSet<LoanApplication>();
            loanApplication = loanApplicationSet.Find(laId);
            var doc = loanApplication.PersonalData;
            var customerManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(Context));
            var customers = Context.Set<Customer>();
            var customer = customers
                .SingleOrDefault(c => c.PersonalData.Identification == doc.Identification);
            if (customer == null)
            {
                customer = new Customer
                {
                    PersonalData = doc,
                    Email = loanApplication.Email,
                    Id = Guid.NewGuid().ToString(),
                    Phone = loanApplication.CellPhone,
                    UserName = CustomerHelper.GenerateName(customerManager, doc),
                };
                password = System.Web.Security.Membership.GeneratePassword(10, 1);
                customerManager.Create(customer, password);
                customerManager.AddToRole(customer.Id, "Customer");
            }
            else
            {
                // TODO: check doc.Id
                customer.PersonalData = doc;
            }
            loanApplication = Service.Find<LoanApplication>(laId);
            switch (loanApplication.Status)
            {
                case LoanApplicationStatus.Approved:
                    var loan = Service.CreateLoanContract(customer, loanApplication, User.Identity.GetUserId());
                    if (loan == null)
                    {
                        return HttpNotFound("Failed to create loan");
                    }
                    var pdfViewModel = new LoanPdfViewModel { UserName = customer.UserName, Password = password, Loan = loan };
                    var pdfResult = new PdfResult(pdfViewModel, "Pdf");
                    pdfResult.ViewBag.Title = "PROebank credentials";
                    return pdfResult;
                case LoanApplicationStatus.ContractPrinted:
                    Service.SignLoanContract(loanApplication.Id);
                    return RedirectToAction("Index");   // TODO: maybe another page
                default:
                    return RedirectToAction("Index");
            }
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult PrintContract(LoanApplication loanApplication)
        {
            if (loanApplication == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var laId = loanApplication.Id;
            loanApplication = Service.Find<LoanApplication>(laId);

            var pdfResult = new PdfResult(loanApplication, "PdfContract");
            pdfResult.ViewBag.Title = "PROebank loan contract";
            return pdfResult;
        }

        [Authorize(Roles = "Department head, Consultant. Security")]
        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loan = Service.Find<Loan>(id);
            if (loan == null)
            {
                return View();
            }
            var viewModel = new LoanDetailsViewModel(loan)
            {
                Customer = UnitOfWork.Context.Set<Customer>().Find(loan.CustomerId),
                CanBeClosed = Service.CanLoanBeClosed(loan)
            };
            return View(viewModel);
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult Close(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loan = Service.Find<Loan>(id);
            if (loan == null)
            {
                return RedirectToAction("Index");
            }
            Service.CloseLoan(loan);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Department head")]
        public ActionResult All(int? page, string searchBy, string search, string sortBy)
        {
            var customers = UnitOfWork.Context.Set<Customer>();
            var loans = Service.GetLoans()
                               .ToList()
                               .Select(l => CreateLoanViewModel(l, customers))
                               .AsQueryable();

            loans = Searching(searchBy, search, loans);
            loans = Sorting(sortBy, loans);

            //for sortBy
            ViewBag.NextSortCustomerParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_CUSTOMER_ASC)) ? SORTBY_CUSTOMER_DESC : SORTBY_CUSTOMER_ASC;
            ViewBag.NextSortIsClosedParameter = (sortBy != null && sortBy.Equals(SORTBY_ISCLOSED_ASC)) ? SORTBY_ISCLOSED_DESC : SORTBY_ISCLOSED_ASC;
            ViewBag.NextSortTariffParameter = (sortBy != null && sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;


            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "all";
            return View("Index", loans.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult Active(int? page, string searchBy, string search, string sortBy)
        {
            var customers = UnitOfWork.Context.Set<Customer>();
            var loans = Service.GetLoans()
                .Where(l => !l.IsClosed)
                .ToList()
                .Select(l => CreateLoanViewModel(l, customers))
                .AsQueryable();

            loans = Searching(searchBy, search, loans);
            loans = Sorting(sortBy, loans);

            //for sortBy
            ViewBag.NextSortCustomerParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_CUSTOMER_ASC)) ? SORTBY_CUSTOMER_DESC : SORTBY_CUSTOMER_ASC;
            ViewBag.NextSortTariffParameter = (sortBy != null && sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;


            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "active";
            return View("Index", loans.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Department head, Security")]
        public ActionResult InTrouble(int? page, string searchBy, string search, string sortBy)
        {
            var customers = UnitOfWork.Context.Set<Customer>();
            var today = Service.GetCurrentDate();
            var loans = Service.GetLoans()
                .Where(l => !l.IsClosed && today.Date > l.PaymentSchedule.Payments.Max(p => p.ShouldBePaidBefore))
                .ToList()
                .Select(l => CreateLoanViewModel(l, customers))
                .AsQueryable();

            loans = Searching(searchBy, search, loans);
            loans = Sorting(sortBy, loans);

            //for sortBy
            ViewBag.NextSortCustomerParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_CUSTOMER_ASC)) ? SORTBY_CUSTOMER_DESC : SORTBY_CUSTOMER_ASC;
            ViewBag.NextSortIsClosedParameter = (sortBy != null && sortBy.Equals(SORTBY_ISCLOSED_ASC)) ? SORTBY_ISCLOSED_DESC : SORTBY_ISCLOSED_ASC;
            ViewBag.NextSortTariffParameter = (sortBy != null && sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;


            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "introuble";
            return View("Index", loans.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Closed(int? page, string searchBy, string search, string sortBy)
        {
            var customers = UnitOfWork.Context.Set<Customer>();
            var loans = Service.GetLoans()
                .Where(l => l.IsClosed)
                .ToList()
                .Select(l => CreateLoanViewModel(l, customers))
                .AsQueryable();

            loans = Searching(searchBy, search, loans);
            loans = Sorting(sortBy, loans);

            //for sortBy
            ViewBag.NextSortCustomerParameter = (string.IsNullOrEmpty(sortBy) || sortBy.Equals(SORTBY_CUSTOMER_ASC)) ? SORTBY_CUSTOMER_DESC : SORTBY_CUSTOMER_ASC;
            ViewBag.NextSortTariffParameter = (sortBy != null && sortBy.Equals(SORTBY_TARIFF_ASC)) ? SORTBY_TARIFF_DESC : SORTBY_TARIFF_ASC;


            //for dropdownlist for searching Criteria
            var items = new List<SelectListItem>
                {
                    new SelectListItem() {Text = SEARCHBY_TARIFF, Value = SEARCHBY_TARIFF},
                    new SelectListItem() {Text = SEARCHBY_CELLPHONE, Value = SEARCHBY_CELLPHONE},
                    new SelectListItem() {Text = SEARCHBY_FIRST_NAME, Value = SEARCHBY_FIRST_NAME},
                    new SelectListItem() {Text = SEARCHBY_LAST_NAME, Value = SEARCHBY_LAST_NAME},
                    new SelectListItem() {Text = SEARCHBY_PASSPORT_NUMBER, Value = SEARCHBY_PASSPORT_NUMBER},
                    new SelectListItem() {Text = SEARCHBY_IDENTIFICATIONNUMBER, Value = SEARCHBY_IDENTIFICATIONNUMBER}
                };
            ViewBag.SearchByList = items;

            ViewBag.ActiveTab = "closed";
            return View("Index", loans.ToPagedList(page ?? 1, PAGE_SIZE));
        }

        private static LoanWithCustomerViewModel CreateLoanViewModel(Loan l, IQueryable<Customer> customers)
        {
            return new LoanWithCustomerViewModel { Loan = l, Customer = customers.FirstOrDefault(c => c.Id == l.CustomerId) };
        }


        private static IQueryable<LoanWithCustomerViewModel> Searching(string searchBy, string search, IQueryable<LoanWithCustomerViewModel> loansWithCustomers)
        {
            var loansWithCustomersResults = loansWithCustomers;

            switch (searchBy)
            {
                case SEARCHBY_TARIFF:
                    loansWithCustomersResults = loansWithCustomersResults.Where(lc => lc.Loan.Application.Tariff.Name.Contains(search) || search == null);
                    break;
                case SEARCHBY_CELLPHONE:
                    loansWithCustomersResults = loansWithCustomersResults.Where(lc => lc.Loan.Application.CellPhone.Contains(search) || search == null);
                    break;
                case SEARCHBY_IDENTIFICATIONNUMBER:
                    loansWithCustomersResults =
                        loansWithCustomersResults.Where(lc => lc.Customer.PersonalData.Identification.Contains(search) || search == null);
                    break;
                case SEARCHBY_FIRST_NAME:
                    loansWithCustomersResults = loansWithCustomersResults.Where(lc => lc.Customer.PersonalData.FirstName.Contains(search) || search == null);
                    break;
                case SEARCHBY_LAST_NAME:
                    loansWithCustomersResults = loansWithCustomersResults.Where(lc => lc.Customer.PersonalData.LastName.Contains(search) || search == null);
                    break;
                case SEARCHBY_PASSPORT_NUMBER:
                    loansWithCustomersResults = loansWithCustomersResults.Where(lc => lc.Customer.PersonalData.Passport.Contains(search) || search == null);
                    break;
                case SEARCHBY_STATUS:
                    loansWithCustomersResults =
                        loansWithCustomersResults.ToList()
                        .Where(lc => lc.Loan.Application.Status.ToString().Contains(search) || search == null)
                        .AsQueryable();
                    break;
            }
            return loansWithCustomersResults;
        }

        private static IQueryable<LoanWithCustomerViewModel> Sorting(string sortBy, IQueryable<LoanWithCustomerViewModel> loansWithCustomers)
        {
            var loansWithCustomersResults = loansWithCustomers;

            switch (sortBy)
            {
                case SORTBY_TARIFF_ASC:
                    loansWithCustomersResults = loansWithCustomersResults.OrderBy(lc => lc.Loan.Application.Tariff.Name);
                    break;
                case SORTBY_TARIFF_DESC:
                    loansWithCustomersResults = loansWithCustomersResults.OrderByDescending(lc => lc.Loan.Application.Tariff.Name);
                    break;
                case SORTBY_ISCLOSED_ASC:
                    loansWithCustomersResults = loansWithCustomersResults.OrderBy(lc => lc.Loan.IsClosed);
                    break;
                case SORTBY_ISCLOSED_DESC:
                    loansWithCustomersResults = loansWithCustomersResults.OrderByDescending(lc => lc.Loan.IsClosed);
                    break;
                case SORTBY_CUSTOMER_ASC:
                    loansWithCustomersResults =
                        loansWithCustomersResults.OrderBy(lc => lc.Customer.PersonalData.LastName);
                    break;
                case SORTBY_CUSTOMER_DESC:
                    loansWithCustomersResults =
                        loansWithCustomersResults.OrderByDescending(lc => lc.Customer.PersonalData.LastName);
                    break;
                default: //if sortBy == empty = OrderByDefault => orderBy Customer ASC
                    loansWithCustomersResults = loansWithCustomersResults.OrderBy(lc => lc.Customer.PersonalData.LastName);
                    break;
            }
            return loansWithCustomersResults;
        }
    }
}
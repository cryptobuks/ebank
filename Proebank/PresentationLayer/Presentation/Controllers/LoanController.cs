using System;
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

namespace Presentation.Controllers
{
    public class LoanController : BaseController
    {
        //Amount of elements to display on one page of PagedList
        private const int PAGE_SIZE = 5;

        [Dependency]
        protected ProcessingService Service { get; set; }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult Index(int? page)
        {
            var customers = UnitOfWork.Context.Set<Customer>();
            var loans = Service.GetLoans()
                .Select(l => new LoanWithCustomerViewModel { Loan = l, Customer = customers.FirstOrDefault(c => c.Id == l.CustomerId)})
                .ToList();
            ViewBag.ActiveTab = "Index";
            return View(loans.ToPagedList(page?? 1,PAGE_SIZE));
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
            if (loanApplication.Status != LoanApplicationStatus.Approved)
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

        [Authorize(Roles = "Department head, Consultant")]
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
    }
}
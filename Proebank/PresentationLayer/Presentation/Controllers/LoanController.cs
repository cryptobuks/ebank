using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Web.Mvc;
using Domain;
using Domain.Enums;
using Domain.Models.Customers;
using Domain.Models.Loans;
using Application;
using System.Net;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Presentation.Extensions;
using Presentation.Models;
using RazorPDF;

namespace Presentation.Controllers
{
    public class LoanController : BaseController
    {
        private ProcessingService _processingService;

        public LoanController()
        {
            _processingService = new ProcessingService();
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult Index()
        {
            var loans = _processingService.GetLoans().ToList();
            return View(loans);
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult Preview(Guid? loanApplicationId)
        {
            if (loanApplicationId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _processingService.GetLoanApplications().SingleOrDefault(la => la.Id.Equals(loanApplicationId.Value));
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
            _processingService.Dispose();
            Customer customer = null;
            String password = null;
            // check customer here because of using default UserStore and UserManager
            using (var ctx = new DataContext())
            {
                loanApplication = ctx.Set<LoanApplication>().Find(laId);
                var doc = loanApplication.PersonalData;
                var customerManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(ctx));
                var customers = ctx.Set<Customer>();
                customer = customers
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
                ctx.SaveChanges();
            }
            _processingService = new ProcessingService();
            loanApplication = _processingService.Find<LoanApplication>(laId);
            if (loanApplication.Status == LoanApplicationStatus.Approved)
            {
                var loan = _processingService.CreateLoanContract(customer, loanApplication);
                if (loan == null)
                {
                    return HttpNotFound("Failed to create loan");
                }
                var pdfResult = new PdfResult(new LoanPdfViewModel { UserName = customer.UserName, Password = password, Loan = loan},
                    "Pdf");
                pdfResult.ViewBag.Title = "PROebank credentials";
                return pdfResult;
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult PrintContract(LoanApplication loanApplication)
        {
            if (loanApplication == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var laId = loanApplication.Id;
            loanApplication = _processingService.GetLoanApplications().Single(la => la.Id.Equals(laId));

            var pdfResult = new PdfResult(loanApplication, "PdfContract");
            pdfResult.ViewBag.Title = "PROebank loan contract";
            return pdfResult;
            //return RedirectToAction("Index", loan);
        }

        public ActionResult Details(Guid? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loan = _processingService.GetLoans()
                .Single(l => l.Id == id);
            if (loan == null)
            {
                return View();
                //return HttpNotFound("There is no loan with such Id");
            }
            return View(loan);
        }
    }
}
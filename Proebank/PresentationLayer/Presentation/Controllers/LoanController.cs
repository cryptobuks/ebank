using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application.LoanProcessing;
using Domain.Enums;
using Microsoft.Practices.Unity;
using Domain.Models.Loans;
using Application;
using System.Net;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Domain.Models.Customers;
using Domain;

namespace Presentation.Controllers
{
    public class LoanController : BaseController
    {
        private ProcessingService _processingService { get; set; }

        public LoanController()
        {
            // TODO: remove something or create loan service property
            _processingService = Container.Resolve<ProcessingService>();
        }

        public ActionResult Index()
        {
            var loans = _processingService.GetLoans(la => true);
            return View(loans);
        }

        public ActionResult Preview(Guid? loanApplicationId)
        {
            if (loanApplicationId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var loanApplication = _processingService.GetLoanApplications(la => la.Id.Equals(loanApplicationId.Value)).SingleOrDefault();
            if (loanApplication == null || loanApplication.Status != LoanApplicationStatus.Approved)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(loanApplication);
        }

        public ActionResult Sign(LoanApplication loanApplication)
        {
            if (loanApplication == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            loanApplication = _processingService.GetLoanApplications(la => la.Id.Equals(loanApplication.Id)).Single();

            // check customer here because of using default UserStore and UserManager
            var userStore = new UserStore<Customer>(new DataContext());
            var userManager = new UserManager<Customer>(userStore);
            var user = new Customer
            {
                // TODO: get username from view!
                UserName = "Username" + DateTime.Now.ToFileTime(),
                Address = "No.Address",
                BirthDate = DateTime.Today,
                Email = "no@ema.il",
                FirstName = "Firstname",
                LastName = "Lastname",
                MiddleName = "Middlename",
                IdentificationNumber = "0000-0000-0000"
            };
            var identityResult = userManager.Create(user, "11111111");
            if (!identityResult.Succeeded)
            {
                return HttpNotFound("Failed to create user");
            }

            var loan = _processingService.CreateLoanContract(user, loanApplication);
            if (loan == null)
            {
                return HttpNotFound("Failed to create loan");
            }
            return RedirectToAction("Index", loan);
        }
    }
}
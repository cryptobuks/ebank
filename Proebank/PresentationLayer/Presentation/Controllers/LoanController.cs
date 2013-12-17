﻿using System;
using System.Linq;
using System.Web.Mvc;
using Domain.Enums;
using Domain.Models.Loans;
using Application;
using System.Net;

namespace Presentation.Controllers
{
    public class LoanController : BaseController
    {
        private readonly ProcessingService _processingService;

        public LoanController()
        {
            _processingService = new ProcessingService();
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult Index()
        {
            var loans = _processingService.GetLoans();
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
            if (loanApplication == null || loanApplication.Status != LoanApplicationStatus.Approved)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View(loanApplication);
        }

        [Authorize(Roles = "Department head, Consultant")]
        public ActionResult Sign(LoanApplication loanApplication)
        { 
            if (loanApplication == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var laId = loanApplication.Id;
            loanApplication = _processingService.GetLoanApplications().Single(la => la.Id.Equals(laId));

            // check customer here because of using default UserStore and UserManager
            var doc = loanApplication.PersonalData;
            var loan = _processingService.CreateLoanContract(doc.Customer, loanApplication);
            if (loan == null)
            {
                return HttpNotFound("Failed to create loan");
            }
            return RedirectToAction("Index", loan);
        }
    }
}
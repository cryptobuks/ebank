using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application.LoanProcessing;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class LoanCalculatorController : BaseController
    {
        private static string _cellPhone;

        [Dependency]
        protected ProcessingService Service { get; set; }

        [AllowAnonymous]
        public ActionResult Index()
        {
            var loanCalculatorModel = new LoanCalculatorModel();

            if (TempData["loanApplication"] != null)
            {
                var loanApplication = (LoanApplication)TempData["loanApplication"];
                loanCalculatorModel.LoanAmount = loanApplication.LoanAmount;
                loanCalculatorModel.Term = loanApplication.Term;
                loanCalculatorModel.TariffId = loanApplication.TariffId;
                //cell phone isn't used in Loan Calculator, but required
                _cellPhone = loanApplication.CellPhone;
            }
            else
            {
                _cellPhone = null;
            }
            
            var tariffs = Service.GetTariffs().Where(t => t.IsActive);
            ViewBag.Tariffs = new SelectList(tariffs, "Id", "Name");
            return View(loanCalculatorModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public ActionResult Index(LoanCalculatorModel loanCalculatorModel, string btnToApplication)
        {
            if (btnToApplication != null && btnToApplication == "To Application")
            {
                if (ModelState.IsValid)
                {
                    var loanApplication = new LoanApplication
                        {
                            LoanAmount = loanCalculatorModel.LoanAmount,
                            Term = loanCalculatorModel.Term,
                            TariffId = loanCalculatorModel.TariffId
                        };
                    if (_cellPhone != null)
                        loanApplication.CellPhone = _cellPhone;

                    TempData.Add("loanApplication", loanApplication);
                    return RedirectToAction((User.IsInRole("Consultant") ? "Fill" : "Create"), "LoanApplication");
                }
            }

            var tariffs = Service.GetTariffs().Where(t => t.IsActive).ToList();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
            var tariff = tariffs.FirstOrDefault(t => t.Id == loanCalculatorModel.TariffId);

            if (ModelState.IsValid)
            {
                try
                {
                    var paymentSchedule =
                        PaymentScheduleCalculator.Calculate(loanCalculatorModel.LoanAmount, tariff,
                                                            loanCalculatorModel.Term);
                    loanCalculatorModel.Payments = paymentSchedule.Payments;
                }
                catch (ArgumentException e)
                {
                    ModelState.AddModelError("TariffId", e.Message);
                    return View(loanCalculatorModel);
                }
            }
            return View(loanCalculatorModel);
        }
    }
}
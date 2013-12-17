using Application;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application.LoanProcessing;
using Domain.Models.Loans;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class LoanCalculatorController : BaseController
    {
        private readonly ProcessingService _processingService;

        public LoanCalculatorController()
        {
            _processingService = new ProcessingService();
        }


        [AllowAnonymous]
        public ActionResult Index()
        {
            var loanCalculatorModel = new LoanCalculatorModel();
            
            if (TempData["loanApplication"] != null)
            {
                var loanApplication = (LoanApplication) TempData["loanApplication"];
                loanCalculatorModel.LoanAmount = loanApplication.LoanAmount;
                loanCalculatorModel.Term = loanApplication.Term;
                loanCalculatorModel.TariffId = loanApplication.TariffId;
            }
            
            var tariffs = _processingService.GetTariffs();
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
                    TempData.Add("loanApplication",loanApplication);
                    return RedirectToAction("Create", "LoanApplication");
                }
            }
            var tariffs = _processingService.GetTariffs();
            ViewBag.TariffId = new SelectList(tariffs, "Id", "Name");
            var tariff = tariffs.FirstOrDefault(t => t.Id == loanCalculatorModel.TariffId);

            if (ModelState.IsValid)
            {
                try
                {
                    var paymentSchedule =
                        PaymentScheduleCalculator.CalculatePaymentScheduleWithoutDateTime(loanCalculatorModel.LoanAmount,
                                                                                          tariff,
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

        //[AllowAnonymous]
        //public ActionResult Applying(LoanApplication loanApplication)
        //{
        //    TempData.Add("loanApplication",loanApplication);
        //    return RedirectToAction("Index",loanApplication);
        //}


        //public ActionResult Index()
        //{

        //    var loanCalculatorModel = new LoanCalculatorModel();
        //    var tariffs = _processingService.GetTariffs();
        //    loanCalculatorModel.Tariffs = new SelectList(tariffs, "Id", "Name");
        //    return View(loanCalculatorModel);
        //}


        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Index(LoanCalculatorModel loanCalculatorModel)
        //{
        //    var tariffs = _processingService.GetTariffs();
        //    var tariffsList = tariffs as IList<Tariff> ?? tariffs.ToList();
        //    loanCalculatorModel.Tariffs = new SelectList(tariffsList, "Id", "Name");
        //    var tariff = tariffsList.FirstOrDefault(t => t.Id == loanCalculatorModel.TariffId);

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            var paymentSchedule =
        //                PaymentScheduleCalculator.CalculatePaymentScheduleWithoutDateTime(loanCalculatorModel.Sum,
        //                                                                                  tariff,
        //                                                                                  loanCalculatorModel.Term);
        //            loanCalculatorModel.Payments = paymentSchedule.Payments;
        //        }
        //        catch (ArgumentException e)
        //        {
        //            ModelState.AddModelError("TariffId", e.Message);
        //            return View(loanCalculatorModel);
        //        }
        //    }
        //    return View(loanCalculatorModel);
        //}
    }
}
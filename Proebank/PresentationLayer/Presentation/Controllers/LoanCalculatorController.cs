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

        public ActionResult Index()
        {
            var loanCalculatorModel = new LoanCalculatorModel();
            var tariffs = _processingService.GetTariffs();
            loanCalculatorModel.Tariffs = new SelectList(tariffs, "Id", "Name");
            return View(loanCalculatorModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(LoanCalculatorModel loanCalculatorModel)
        {
            var tariffs = _processingService.GetTariffs();
            var tariffsList = tariffs as IList<Tariff> ?? tariffs.ToList();
            loanCalculatorModel.Tariffs = new SelectList(tariffsList, "Id", "Name");
            var tariff = tariffsList.FirstOrDefault(t => t.Id == loanCalculatorModel.TariffId);
            
            if (ModelState.IsValid)
            {
                try
                {
                    var schedule =
                        PaymentScheduleCalculator.Calculate(loanCalculatorModel.Sum, tariff, loanCalculatorModel.Term);
                    loanCalculatorModel.Payments = schedule.Payments;
                }
                catch (ArgumentException e)
                {
                    ModelState.AddModelError("TariffId", e.Message);
                    return View(loanCalculatorModel);
                }
            }
            return View(loanCalculatorModel);
        }

        //[HttpPost]
        //public ActionResult PaymentSchedule(string sumTextBox, string termTextBox, string Tariffs)
        //{
        //    var sum = Convert.ToDecimal(sumTextBox);
        //    var term = Convert.ToInt32(termTextBox);

        //    var tariffId = Guid.Parse(Tariffs);
        //    var tariff = _processingService.GetTariffs(t => t.Id == tariffId).Single();

        //    var paymentSchedule = PaymentScheduleCalculator.CalculateWithoutDate(sum, tariff, term);


        //    return View(paymentSchedule.Payments);
        //}
    }
}
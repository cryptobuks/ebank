using System.Linq;
using System.Web.Mvc;
using Application;
using Microsoft.Practices.Unity;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class AtmController : BaseController
    {
        [Dependency]
        protected ProcessingService Service { get; set; }

        [Authorize(Roles = "Operator")]
        public ActionResult Index()
        {
            var loans = Service.GetLoans();
            var nameLoan = loans.Select(l => new
            {
                Name = l.Application.PersonalData.FirstName + " " + l.Application.PersonalData.LastName + " (" + l.Application.Tariff.Name + ")",
                Id = l.Id
            }).ToList();
            ViewBag.LoanId = new SelectList(nameLoan, "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Operator")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AtmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loan = Service.GetLoans().FirstOrDefault(l => l.Id == model.LoanId);
                if (loan != null)
                {
                    Service.RegisterPayment(loan, model.Amount);
                }
                else
                {
                    ModelState.AddModelError("LoanId", "Loan is not found");
                }
            }
            ViewBag.PaymentRegistered = true;
            return RedirectToAction("Index");
        }
    }
}
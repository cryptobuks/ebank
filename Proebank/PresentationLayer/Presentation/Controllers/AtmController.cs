using System.Linq;
using System.Web.Mvc;
using Application;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class AtmController : BaseController
    {
        private readonly ProcessingService _service;

        public AtmController()
        {
            _service = new ProcessingService();
        }

        [Authorize(Roles = "Operator")]
        public ActionResult Index()
        {
            var loans = _service.GetLoans();
            ViewBag.LoanId = new SelectList(loans, "Id", "Id");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Operator")]
        [ValidateAntiForgeryToken]
        public ActionResult Index(AtmViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loan = _service.GetLoans().FirstOrDefault(l => l.Id == model.LoanId);
                if (loan != null)
                {
                    _service.RegisterPayment(loan, model.Amount);
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
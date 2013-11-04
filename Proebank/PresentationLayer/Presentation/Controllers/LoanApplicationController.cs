using Domain.Models.Accounts;
using Domain.Models.Loans;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class LoanApplicationController : BaseController
    {
        private ILoanApplicationRepository Repository { get; set; }

        public LoanApplicationController()
        {
            Repository = Container.Resolve<ILoanApplicationRepository>();
        }

        public ActionResult Index()
        {
            var loans = Repository.GetAll();
            return View(loans);
        }
    }
}
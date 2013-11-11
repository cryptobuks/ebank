using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Application.LoanProcessing;
using Microsoft.Practices.Unity;

namespace Presentation.Controllers
{
    public class LoanController : BaseController
    {
        private LoanService LoanService { get; set; }

        public LoanController()
        {
            LoanService = Container.Resolve<LoanService>();
        }

        public ActionResult Index()
        {
            var loans = LoanService.GetAll();
            return View(loans.ToList());    // TODO: why ToList() is needed?
        }
    }
}
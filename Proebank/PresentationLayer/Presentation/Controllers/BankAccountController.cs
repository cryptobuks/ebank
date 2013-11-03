using Domain.Models.Accounts;
using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class BankAccountController : BaseController
    {
        public IAccountRepository Repository { get; set; }

        public BankAccountController()
        {
            Repository = Container.Resolve<IAccountRepository>();
        }

        public ActionResult Index()
        {
            var accounts = Repository.GetAll();
            return View(accounts);
        }
    }
}
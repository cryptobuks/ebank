using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        public IUnityContainer Container { get; set; }

        public BaseController()
        {
            Container = new UnityContainer();
            Container.LoadConfiguration();
        }
    }
}
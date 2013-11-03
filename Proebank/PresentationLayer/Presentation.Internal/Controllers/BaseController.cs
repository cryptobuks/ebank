using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Presentation.Internal.Controllers
{
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
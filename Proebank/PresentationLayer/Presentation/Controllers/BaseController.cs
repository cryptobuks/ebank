﻿using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System.Web.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    public abstract class BaseController : Controller
    {
        //public IUnityContainer Container;

        //public BaseController()
        //{
        //    Container = new UnityContainer();
        //    Container.LoadConfiguration();
        //}
    }
}
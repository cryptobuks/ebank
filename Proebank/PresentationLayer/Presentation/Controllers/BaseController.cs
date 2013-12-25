using Domain;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using System;
using System.Web.Mvc;

namespace Presentation.Controllers
{

    [Authorize]
    public abstract class BaseController : Controller
    {
        [Dependency]
        protected Lazy<IUnitOfWork> UnitOfWork { get; set; }
    }
}
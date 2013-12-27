using Domain;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Practices.Unity;
using System;
using System.Web.Mvc;

namespace Presentation.Controllers
{

    [Authorize]
    public abstract class BaseController : Controller
    {
        [Dependency]
        protected Lazy<UnitOfWork> LazyUnitOfWork { get; set; } // private will fail unity

        protected UnitOfWork UnitOfWork { get { return LazyUnitOfWork.Value; } }

        protected IdentityDbContext<IdentityUser> Context { get { return UnitOfWork.Context; } }
    }
}
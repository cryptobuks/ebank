using Domain;
using Microsoft.AspNet.Identity.EntityFramework;
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
        protected Lazy<IUnitOfWork> LazyUnitOfWork { get; set; }

        protected IUnitOfWork UnitOfWork { get { return LazyUnitOfWork.Value; } }

        protected IdentityDbContext<IdentityUser> Context { get { return UnitOfWork.Context; } }
    }
}
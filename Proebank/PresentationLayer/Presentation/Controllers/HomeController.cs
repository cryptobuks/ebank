using System.Web.Mvc;
using System.Web.Security;

namespace Presentation.Controllers
{
    [AllowAnonymous]
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "PROebank description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "PROebank contact page.";

            return View();
        }

        [ChildActionOnly]
        public ActionResult MainMenu()
        {
            //ViewBag.Roles = Roles.GetRolesForUser();
            return View("~/Views/Shared/MainMenu.cshtml");
        }
    }
}
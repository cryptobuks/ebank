using System.Web.Mvc;

namespace Presentation.Controllers
{
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
    }
}
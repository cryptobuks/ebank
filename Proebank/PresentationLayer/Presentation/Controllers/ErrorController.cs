using System.Web.Mvc;

namespace Presentation.Controllers
{
    public class ErrorController : Controller
    {
        //
        // GET: /Error/
        [AllowAnonymous]
        public ActionResult Index()
        {
            return RedirectToAction("Index", "Home");
        }

        [AllowAnonymous]
        public ActionResult NotFound()
        {
            Response.StatusCode = 404;
            return View();
        }

        [AllowAnonymous]
        public ActionResult BadRequest()
        {
            Response.StatusCode = 400;
            return View();
        }

        [AllowAnonymous]
        public ActionResult ServerError()
        {
            return View();
        }
	}
}
using System.Web.Mvc;
using Application;

namespace Presentation.Controllers
{
    [Authorize]
    public class ProcessingController : BaseController
    {
        private readonly ProcessingService _service;

        public ProcessingController()
        {
            // TODO: remove something or create loan service property
            _service = new ProcessingService();
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Index()
        {
            var time = _service.GetCurrentDate();
            return View(time);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult MoveNextDay()
        {
            var newDate = _service.ProcessEndOfDay();
            return RedirectToAction("Index", newDate);
        }
    }
}
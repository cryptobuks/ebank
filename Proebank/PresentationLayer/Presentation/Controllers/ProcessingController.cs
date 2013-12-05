using System.Web.Mvc;
using Application;
using Microsoft.Practices.Unity;

namespace Presentation.Controllers
{
    [Authorize]
    public class ProcessingController : BaseController
    {
        private ProcessingService _service { get; set; }

        public ProcessingController()
        {
            // TODO: remove something or create loan service property
            _service = Container.Resolve<ProcessingService>();
        }

        public ActionResult Index()
        {
            var time = _service.GetCurrentDate();
            return View(time);
        }

        public ActionResult MoveNextDay()
        {
            var newDate = _service.ProcessEndOfDay();
            return RedirectToAction("Index", newDate);
        }
    }
}
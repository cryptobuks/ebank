using System.Web.Mvc;
using Application;
using Microsoft.Practices.Unity;

namespace Presentation.Controllers
{
    [Authorize]
    public class ProcessingController : BaseController
    {
        private ProcessingService _processingService { get; set; }

        public ProcessingController()
        {
            // TODO: remove something or create loan service property
            _processingService = Container.Resolve<ProcessingService>();
        }

        public ActionResult Index()
        {
            var time = _processingService.GetCurrentTime();
            return View(time);
        }

        public ActionResult MoveNextDay()
        {
            var newDate = _processingService.ProcessEndOfDay();
            return RedirectToAction("Index", newDate);
        }
    }
}
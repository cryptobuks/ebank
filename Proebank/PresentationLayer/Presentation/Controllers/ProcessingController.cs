using System;
using System.ComponentModel.DataAnnotations;
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

        [Authorize(Roles = "Department head")]
        public ActionResult MoveNextWeek()
        {
            var newDate = new DateTime();
            for (var i = 0; i < 7; i++)
            {
                newDate = _service.ProcessEndOfDay();
            }
            return RedirectToAction("Index", newDate);
        }
    }
}
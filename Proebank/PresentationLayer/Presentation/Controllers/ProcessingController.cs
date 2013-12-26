using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Application;
using Microsoft.Practices.Unity;

namespace Presentation.Controllers
{
    [Authorize]
    public class ProcessingController : BaseController
    {
        [Dependency]
        protected ProcessingService Service { get; set; }

        [Authorize(Roles = "Department head")]
        public ActionResult Index()
        {
            
            var time = Service.GetCurrentDate();
            return View(time);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult MoveNextDay()
        {
            
            var newDate = Service.ProcessEndOfDay();
            return RedirectToAction("Index", newDate);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult MoveNextWeek()
        {
            
            var newDate = new DateTime();
            for (var i = 0; i < 7; i++)
            {
                newDate = Service.ProcessEndOfDay();
            }
            return RedirectToAction("Index", newDate);
        }
    }
}
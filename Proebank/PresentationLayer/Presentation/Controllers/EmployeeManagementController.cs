using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Domain.Models.Users;
using Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class EmployeeManagementController : BaseController
    {
        private readonly DataContext _ctx = new DataContext();

        // GET: /EmployeeManagement/
        [Authorize(Roles = "Department head")]
        public ActionResult Index()
        {
            return View(_ctx.Employees.ToList());
        }

        // GET: /EmployeeManagement/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = _ctx.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: /EmployeeManagement/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /EmployeeManagement/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="Id,UserName,LastName,FirstName,MiddleName,EmployeeRole")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(_ctx));
                var password = GeneratePassword();
                employee.HiredOn = DateTime.UtcNow;
                var userResult = userManager.Create(employee, password);
                if (userResult.Succeeded)
                {
                    _ctx.SaveChanges();
                    Debug.WriteLine("Generated password is: " + password);
                    return EmployeeCreated(new CreatedEmployeeViewModel {Employee = employee, Password = password});
                }
                ModelState.AddModelError("", String.Join("\n", userResult.Errors));
            }

            return View(employee);
        }

        private string GeneratePassword()
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (var i = 0; i < 12; i++)
            {
                var ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            return builder.ToString();
        }

        // GET: /EmployeeManagement/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = _ctx.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: /EmployeeManagement/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="Id,UserName,PasswordHash,SecurityStamp,LastName,FirstName,MiddleName,HiredOn,FiredOn,EmployeeRole")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _ctx.Entry(employee).State = EntityState.Modified;
                _ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(employee);
        }

        // GET: /EmployeeManagement/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = _ctx.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // POST: /EmployeeManagement/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Employee employee = _ctx.Employees.Find(id);
            _ctx.Users.Remove(employee);
            _ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        // POST: /EmployeeManagement/EmployeeCreated/5
        [HttpPost, ActionName("EmployeeCreated")]
        public ActionResult EmployeeCreated(CreatedEmployeeViewModel user)
        {
            if (user == null || user.Employee == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            return View("EmployeeCreated", user);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _ctx.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

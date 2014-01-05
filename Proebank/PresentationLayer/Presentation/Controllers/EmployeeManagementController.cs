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
using Domain.Enums;
using Domain.Models.Users;
using Domain;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Presentation.Models;

namespace Presentation.Controllers
{
    public class EmployeeManagementController : BaseController
    {
        // GET: /EmployeeManagement/
        [Authorize(Roles = "Department head")]
        public ActionResult Index()
        {
            return View(Context.Set<Employee>().ToList());
        }

        // GET: /EmployeeManagement/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = Context.Set<Employee>().Find(id);
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
                var userManager = new UserManager<IdentityUser>(new UserStore<IdentityUser>(Context));
                var password = GeneratePassword();
                employee.HiredOn = DateTime.UtcNow;
                var userResult = userManager.Create(employee, password);
                userManager.AddToRole(employee.Id, ConvertRole(employee.EmployeeRole));
                if (userResult.Succeeded)
                {
                    UnitOfWork.SaveChanges();
                    Debug.WriteLine("Generated password is: " + password);
                    return EmployeeCreated(new CreatedEmployeeViewModel {Employee = employee, Password = password});
                }
                ModelState.AddModelError("", String.Join("\n", userResult.Errors));
            }

            return View(employee);
        }

        private string ConvertRole(EmployeeRole employeeRole)
        {
            switch (employeeRole)
            {
                case EmployeeRole.CreditCommitee:
                    return "Credit committee";
                case EmployeeRole.Chief:
                    return "Department head";
                case EmployeeRole.Consultant:
                    return "Consultant";
                case EmployeeRole.Operator:
                    return "Operator";
                case EmployeeRole.SecurityService:
                    return "Security";
            }
            throw new ArgumentException();
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
            Employee employee = Context.Set<Employee>().Find(id);
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
                Context.Entry(employee).State = EntityState.Modified;
                UnitOfWork.SaveChanges();
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
            var employee = Context.Set<Employee>().Find(id);
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
            Employee employee = Context.Set<Employee>().Find(id);
            Context.Set<IdentityUser>().Remove(employee);
            UnitOfWork.SaveChanges();
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
    }
}

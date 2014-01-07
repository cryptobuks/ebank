using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Domain.Models.Loans;
using Domain;
using Application;
using Microsoft.Practices.Unity;
using Presentation.Models;
using RazorPDF;

namespace Presentation.Controllers
{
    public class StatisticController : BaseController
    {
        [Dependency]
        protected ProcessingService Service { get; set; }

        [Authorize(Roles = "Department head")]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Department head")]
        public ActionResult LoanApplication()
        {
            var list = Service.GetLoanApplications().ToList();
            return View(list);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Loan()
        {
            var list = Service.GetLoans().ToList();
            return View(list);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult Tariff()
        {
            var list = Service.GetLoans().ToList();
            return View(list);
        }

        [Authorize(Roles = "Department head")]
        public ActionResult MonthlyReport()
        {
            var report = Service.GetMonthlyReport();
            var pdfResult = new PdfResult(report, "PdfMonthlyReport");
            pdfResult.ViewBag.Title = "Monthly Report";
            return pdfResult;
        }

        [Authorize(Roles = "Department head")]
        public ActionResult AnnualReport()
        {
            var report = Service.GetAnnualReport();
            var pdfResult = new PdfResult(report, "PdfAnnualReport");
            pdfResult.ViewBag.Title = "Annual Report";
            return pdfResult;
        }
    }
}

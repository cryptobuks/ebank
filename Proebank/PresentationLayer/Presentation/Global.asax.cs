using System;
using System.Web;
using Domain.Contexts;
using Domain.Migrations;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Presentation
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Database.SetInitializer<DbDataContext>(new MigrateDatabaseToLatestVersion<DbDataContext, Configuration>());
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error()
        {
            var exception = Server.GetLastError();
            var httpException = exception as HttpException;

            // TODO: uncomment before release
            //if (httpException != null)
            //{
            //    Response.StatusCode = httpException.GetHttpCode();

            //    Response.Clear();
            //    Server.ClearError();

            //    if (httpException != null)
            //    {
            //        var httpContext = HttpContext.Current;
            //        httpContext.RewritePath("/Error/ServerError", false);

            //        // MVC3+ and IIS7+
            //        switch (Response.StatusCode)
            //        {
            //            case 404:
            //                httpContext.Server.TransferRequest("/Error/NotFound", true);
            //                break;
            //            default:
            //                httpContext.Server.TransferRequest("/Error/ServerError", true);
            //                break;
            //        }
            //    }
            //}
        }
    }
}

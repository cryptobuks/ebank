﻿using System.Web.Optimization;

namespace Presentation
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
                        "~/Scripts/jquery-ui-1.10.3.custom.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/bundles/jquery-ui").Include(
                      "~/Content/jquery-ui-1.10.3.custom.min.css"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css"));

            bundles.Add(new StyleBundle("~/font-awesome").Include(
                "~/Content/font-awesome.min.css"));

            bundles.Add(new ScriptBundle("~/bundles/Chart").Include(
            "~/Scripts/Chart.js"));

            bundles.Add(new ScriptBundle("~/bundles/proebank").Include(
            "~/Scripts/proebank.js",
            "~/Scripts/chance.min.js"));

            bundles.Add(new ScriptBundle("~/amcharts").Include(
            "~/Scripts/amcharts/amcharts.js", "~/Scripts/amcharts/pie.js"));
        }
    }
}

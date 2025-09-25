using System.Web.Optimization;

namespace FBBConfig
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/js/jquery").Include(
                       "~/Scripts/js/jquery.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            //            "~/Scripts/jquery-ui-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/js/jqueryval").Include(
            //            "~/Scripts/jquery.unobtrusive.min.js",
            //            "~/Scripts/jquery.validate.min.js"));

            //bundles.Add(new ScriptBundle("~/bundles/js/modernizr").Include("~/Scripts/modernizr-2.6.2.js"));

            //bundles.Add(new StyleBundle("~/Content/font-awesome-4.1.0").Include(
            //    "~/Content/font-awesome-4.1.0/css/font-awesome.min.css"
            //    ));

            // The Kendo CSS bundle
            bundles.Add(new StyleBundle("~/Content/styles/kendo").Include(
                "~/Content/kendo/2014.1.415/kendo.common.min.css",
                "~/Content/kendo/2014.1.415/kendo.dataviz.min.css",
                 //"~/Content/kendo/2014.1.415/kendo.metro.min.css",
                 "~/Content/kendo/2014.1.415/kendo.dataviz.default.min.css",
                "~/Content/styles/bootstrap.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/css/fbbwc").Include(
             "~/Content/i-style.css",
             "~/Content/menu.css",
             "~/Content/override-bootstrap.css",
             "~/Content/override-kendo.css"));

            bundles.Add(new ScriptBundle("~/bundles/js/fbbwc").Include(
                "~/Scripts/spin.min.js",
                "~/Scripts/bootbox.min.js",
                "~/Scripts/jquery.fileDownload.js",
                "~/Scripts/myscript.js",
                "~/Scripts/i-script.js",
                "~/Scripts/iworkflow.script.js",
                "~/Scripts/jquery.masked-input-1.3.1.js",
                "~/Scripts/icheck.js",
                "~/Scripts/jquery.tooltipster.min.js",
                "~/Scripts/jquery.confirm.js",
                "~/Scripts/jquery.cookie.js"
                //"~/Scripts/resend_order_form.js"
                ));


            // The Kendo JavaScript bundle
            bundles.Add(new ScriptBundle("~/bundles/js/kendo").Include(
                "~/Scripts/kendo/2014.1.415/kendo.all.min.js",
                "~/Scripts/kendo/2014.1.415/kendo.aspnetmvc.min.js",
                "~/Scripts/kendo.modernizr.custom.js"
                //"~/Scripts/js/kendo.web.min.js",
                //"~/Scripts/js/kendo.aspnetmvc.min.js",
                //"~/Scripts/js/cultures/kendo.culture.th-TH.min.js",
                //"~/Scripts/js/cultures/kendo.culture.th.min.js",
                //"~/Scripts/js/cultures/kendo.culture.en-US.min.js",
                //"~/Scripts/js/cultures/kendo.culture.en-GB.min.js"
                ));

            bundles.Add(new ScriptBundle("~/bundles/js/bootstrap").Include("~/Scripts/bootstrap.min.js"));

            //for login pages
            bundles.Add(new ScriptBundle("~/bundles/js/login").Include("~/Scripts/iworkflow.login.js"));
            bundles.Add(new StyleBundle("~/bundles/css/login").Include("~/Content/styles/iworkflow.login.css"));

            bundles.IgnoreList.Clear();

            // Add back the default ignore list rules sans the ones which affect minified files and debug mode
            bundles.IgnoreList.Ignore("*.intellisense.js");
            bundles.IgnoreList.Ignore("*-vsdoc.js");
            bundles.IgnoreList.Ignore("*.debug.js", OptimizationMode.WhenEnabled);
        }
    }
}
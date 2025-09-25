
using System.Web.Optimization;
namespace WBBWeb
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.IgnoreList.Clear();

            #region Rabbit
            bundles.Add(new ScriptBundle("~/bundles/js/jquery19").Include(
                       "~/Scripts/js/jquery.min.js"));

            bundles.Add(new StyleBundle("~/Content/css/rabbit").Include(
             "~/Content/css/bootstrap.min.css",
             "~/Content/css/bootstrap-theme.min.css",
             "~/Content/css/i-rabbit.css"
             ));

            bundles.Add(
                new ScriptBundle("~/bundles/js/rabbit").Include(
                    "~/Scripts/js/bootstrap.min.js",
                    "~/Scripts/bootbox.min.js",
                    "~/Scripts/js/rabbit.js",
                    "~/Scripts/jquery.watermark.min.js"));

            #endregion

            #region FBB

            //#if DEBUG

            //            bundles.Add(
            //                    new StyleBundle("~/bundles/css/2016")
            //                        .Include("~/Content/Content2016/css/*.css", new CssRewriteUrlTransformFixed()));
            //            // .Include("~/Content/Content2016/css/helvetica.css", new CssRewriteUrlTransformFixed()));

            //#else
            //            bundles.Add(
            //                    new StyleBundle("~/bundles/css/2016")
            //                        .Include("~/Content/Content2016/css/*.min.css", new CssRewriteUrlTransform())
            //                        .Include("~/Content/Content2016/css/helvetica.min.css", new CssRewriteUrlTransform()));
            //#endif
            bundles.Add(
            new StyleBundle("~/Content/Content2016/fibrepostpaid").Include("~/Content/Content2016/css/fibrepostpaid.min.css", new CssRewriteUrlTransformFixed()));

            bundles.Add(
               new StyleBundle("~/Content/Content2016/helvetica").Include("~/Content/Content2016/css/helvetica.min.css", new CssRewriteUrlTransformFixed()));


            bundles.Add(
                new StyleBundle("~/bundles/css/2016")
                    .Include("~/Content/Content2016/css/dna.global.min.css",
                    "~/Content/Content2016/css/foundation.min.css",
                    "~/Content/Content2016/css/index.min.css",
                    "~/Content/Content2016/css/jquery.typeahead.min.css",
                    "~/Content/Content2016/css/menuresponsive.css",
                    "~/Content/Content2016/css/menutypeahead.min.css",
                    "~/Content/Content2016/css/navais.min.css"));





            bundles.Add(
                new StyleBundle("~/bundles/css/2016plugin")
                    .Include(
                        "~/Content/Content2016/plugin/sod/selectordie.min.css",
                        "~/Content/Content2016/plugin/sod/selectordie_theme.min.css",
                        "~/Content/Content2016/plugin/slick/slick.min.css",
                        "~/Content/Content2016/plugin/slick/slick-theme.min.css",
                        "~/Content/font-awesome-4.1.0/css/font-awesome.min.css"));

            bundles.Add(
                new StyleBundle("~/Content/css/wbb")
                    .Include(
                        "~/Content/i-style.css",
                        "~/Content/StyleSheet.css",
                        "~/Scripts/popup/basic.css"));

            bundles.Add(new StyleBundle("~/Content/css/bootsrapcss").Include("~/Content/css/bootstrap.min.css"));
            bundles.Add(new StyleBundle("~/Content/css/typeaheadcss").Include("~/Content/typeahead/typeahead.css"));
            bundles.Add(new StyleBundle("~/bundles/css/vendor").Include("~/Content/jquery.realperson.css"));

            bundles.Add(
                new StyleBundle("~/bundles/css/responsivefibre")
                    .Include(
                        "~/css/select2.min.css",
                        "~/css/main.min.css",
                        "~/css/fbbmetbootstrap.min.css",
                        "~/css/toastr.min.css"));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/jquery")
                        .Include(
                            "~/Scripts/js/jquery.min.js",
                            "~/js/select2.min.js",
                            "~/js/jquery.inputmask.bundle.min.js",
                            "~/Content/Content2016/plugin/slick/slick.min.js",
                            "~/Content/Content2016/plugin/sod/selectordie.min.js",
                            "~/Scripts/jquery.watermark.min.js",
                            "~/Scripts/jquery.datetimepicker.min.js"

                            ));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/jquerymigrate")
                    .Include("~/Scripts/js/jquery-migrate.min.js"));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/wbb")
                    .Include(
                        "~/Scripts/spin.min.js",
                        "~/Scripts/bootbox.min.js",
                        "~/Scripts/myscript.min.js",
                        "~/Scripts/i-script.js",
                        "~/Content/Content2016/js/pdf.js",
                        "~/Content/Content2016/js/pdf.worker.js"

                        //"~/Scripts/iworkflow.script.js",
                        //"~/Scripts/icheck.js",
                        //"~/Scripts/jquery.masked-input-1.3.1.js",
                        //"~/Scripts/jquery.tooltipster.min.js",
                        //"~/Scripts/jquery.confirm.js",
                        //"~/Scripts/jquery.cookie.js",
                        //"~/Scripts/ValidateAlphabet.js",
                        //"~/Scripts/validate-alphabet.js",
                        //"~/Scripts/popup/jquery.simplemodal.js",
                        ));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/fibrenet").Include(
                        "~/Scripts/jquery.plugin.min.js",
                        "~/Scripts/jquery.realperson.min.js",
                        "~/Scripts/hmac-sha256.min.js",
                        "~/Scripts/enc-base64-min.js",
                        "~/js/ifbbweb.min.js"));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/boostrapjs")
                        .Include("~/Scripts/js/bootstrap.min.js"));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/typeaheadjs")
                        .Include("~/Scripts/typeahead.js/typeahead.jquery.min.js"));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/responsivefibrejs")
                        .Include(
                            "~/js/toastr.min.js",
                            "~/js/Main.min.js",
                            "~/Content/Content2016/js/dna.global.min.js",
                            "~/Content/Content2016/js/tweenmax.min.js",
                            "~/Content/Content2016/js/menutypeahead.min.js",
                            "~/Content/Content2016/js/jquery.mainnav.min.js",
                            "~/js/vendor/quagga.min.js"
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
            //for login pages
            bundles.Add(new ScriptBundle("~/bundles/js/login").Include("~/Scripts/iworkflow.login.js"));
            bundles.Add(new StyleBundle("~/bundles/css/login").Include("~/Content/css/iworkflow.login.css"));

            // jqueryui for Progressbar on Cardreader
            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include("~/Scripts/jquery-ui.min.js"));
            bundles.Add(new StyleBundle("~/Content/themes/base/css").Include(
              "~/Content/themes/base/jquery.ui.core.css",
              "~/Content/themes/base/jquery.ui.resizable.css",
              "~/Content/themes/base/jquery.ui.selectable.css",
              "~/Content/themes/base/jquery.ui.accordion.css",
              "~/Content/themes/base/jquery.ui.autocomplete.css",
              "~/Content/themes/base/jquery.ui.button.css",
              "~/Content/themes/base/jquery.ui.dialog.css",
              "~/Content/themes/base/jquery.ui.slider.css",
              "~/Content/themes/base/jquery.ui.tabs.css",
              "~/Content/themes/base/jquery.ui.datepicker.css",
              "~/Content/themes/base/jquery.ui.progressbar.css",
              "~/Content/themes/base/jquery.ui.theme.css"));

            // bundles.Add(
            //         new ScriptBundle("~/bundles/js/analytic-staging")
            //         .Include("~/js/analytic/satelliteLib-2756bab2480cb6c2674fb064e707912a7e9432b2-staging.js"));

            // bundles.Add(
            //         new ScriptBundle("~/bundles/js/analytic")
            //         .Include("~/js/analytic/satelliteLib-2756bab2480cb6c2674fb064e707912a7e9432b2.js"));


            bundles.Add(
                    new ScriptBundle("~/bundles/js/qrcodejs")
                        .Include("~/Scripts/js/jquery-qrcode.js", "~/Scripts/js/qrcode2.js"));

            bundles.Add(
                    new ScriptBundle("~/bundles/signalR")
                        .Include("~/Scripts/jquery.signalR-2.4.3.js"));

            #endregion

            #region saleportal
            bundles.Add(new StyleBundle("~/Content/saleportal/styles/kendo").Include(
                "~/Content/saleportal/kendo/2014.1.415/kendo.common.min.css",
                "~/Content/saleportal/kendo/2014.1.415/kendo.dataviz.min.css",
                 //"~/Content/saleportal/kendo/2014.1.415/kendo.metro.min.css",
                 "~/Content/saleportal/kendo/2014.1.415/kendo.dataviz.default.min.css",
                "~/Content/saleportal/styles/bootstrap.min.css"
                ));

            bundles.Add(new StyleBundle("~/Content/saleportal/css/fbbwc").Include(
             "~/Content/saleportal/i-style.css",
             "~/Content/saleportal/menu.css",
             "~/Content/saleportal/override-bootstrap.css",
             "~/Content/saleportal/override-kendo.css"));

            bundles.Add(new ScriptBundle("~/bundles/saleportal/js/fbbwc").Include(
                "~/Scripts/saleportal/spin.min.js",
                "~/Scripts/saleportal/bootbox.min.js",
                "~/Scripts/saleportal/jquery.fileDownload.js",
                "~/Scripts/saleportal/myscript.js",
                "~/Scripts/saleportal/i-script.js",
                "~/Scripts/saleportal/iworkflow.script.js",
                "~/Scripts/saleportal/jquery.masked-input-1.3.1.js",
                "~/Scripts/saleportal/icheck.js",
                "~/Scripts/saleportal/jquery.tooltipster.min.js",
                "~/Scripts/saleportal/jquery.confirm.js",
                "~/Scripts/saleportal/jquery.cookie.js"));

            // The Kendo JavaScript bundle
            bundles.Add(new ScriptBundle("~/bundles/saleportal/js/kendo").Include(
                "~/Scripts/saleportal/kendo/2014.1.415/kendo.all.min.js",
                "~/Scripts/saleportal/kendo/2014.1.415/kendo.aspnetmvc.min.js",
                "~/Scripts/saleportal/kendo.modernizr.custom.js"
                //"~/Scripts/saleportal/js/kendo.web.min.js",
                //"~/Scripts/saleportal/js/kendo.aspnetmvc.min.js",
                //"~/Scripts/saleportal/js/cultures/kendo.culture.th-TH.min.js",
                //"~/Scripts/saleportal/js/cultures/kendo.culture.th.min.js",
                //"~/Scripts/saleportal/js/cultures/kendo.culture.en-US.min.js",
                //"~/Scripts/saleportal/js/cultures/kendo.culture.en-GB.min.js"
                ));
            #endregion

            #region Redesign FBB

            //Add css Redesign FBB
            bundles.Add(new StyleBundle("~/bundles/css/CssRedesign").Include(
             //"~/Content/Content2016/css/fibrepostpaid.css",
             //"~/Content/Content2016/plugin/sod/selectordie.css",
             "~/Content/Content2016/css/menutypeahead.css",
             "~/Content/Content2016/css/jquery.typeahead.css",
             "~/Content/Content2016/css/index.css",
             "~/Content/Content2016/css/navais.min.css",
             "~/Content/Content2016/css/dna.global.css",
             //"~/Content/Content2016/css/foundation.css",
             "~/Content/Content2016/css/helvetica.css",
             "~/Content/font-awesome-4.1.0/css/font-awesome.min.css",
             "~/css/select2.min.css"
             ));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/jsRedesign").Include(
                        "~/Content/Content2016/js/jquery-1.11.3.min.js",
                        //"~/Content/Content2016/js/tweenmax.js",
                        //"~/Content/Content2016/js/dna.global.js",
                        "~/Content/Content2016/js/jquery.mainnav.min.js",
                        //"~/Content/Content2016/plugin/sod/selectordie.js",
                        //"~/Content/Content2016/js/menutypeahead.js"
                        //"~/Content/Content2016/js/jquery.typeahead.min.js",
                        "~/js/select2.min.js",
                        "~/js/jquery.matchHeight.min.js"
                        ));

            bundles.Add(
                    new ScriptBundle("~/bundles/js/jsRedesignResponsive")
                        .Include(
                            "~/js/Main.min.js"
                        ));


            #endregion


        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.UI;

namespace WebApp
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkID=303951
        public static void RegisterBundles(BundleCollection bundles)
        {
            /*
            bundles.Add(new ScriptBundle("~/bundles/WebFormsJs").Include(
                            "~/Scripts/WebForms/WebForms.js",
                            "~/Scripts/WebForms/WebUIValidation.js",
                            "~/Scripts/WebForms/MenuStandards.js",
                            "~/Scripts/WebForms/Focus.js",
                            "~/Scripts/WebForms/GridView.js",
                            "~/Scripts/WebForms/DetailsView.js",
                            "~/Scripts/WebForms/TreeView.js",
                            "~/Scripts/WebForms/WebParts.js"));

            // Order is very important for these files to work, they have explicit dependencies
            bundles.Add(new ScriptBundle("~/bundles/MsAjaxJs").Include(
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjax.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxApplicationServices.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxTimer.js",
                    "~/Scripts/WebForms/MsAjax/MicrosoftAjaxWebForms.js"));

            */
            // css bundle
            bundles.Add(new StyleBundle("~/Content/css").Include(
                "~/Content/font-awesome.min.css",
                "~/Content/bootstrap.min.css",
                "~/Content/navbar_primary.css",
                "~/Content/jquery-ui-1.11.4.min.css",
                "~/Content/jquery-ui-1.11.4.structure.min.css",
                "~/Content/jquery-ui-1.11.4.theme.min.css",
                "~/Content/jexcel.css",
                "~/Content/jsuites.css",
                "~/Content/foundation.css",
                "~/Content/webapp.css")
                );


            // js bundle
            bundles.Add(new ScriptBundle("~/Scripts/js").Include(
                "~/Scripts/jquery-3.5.1.min.js",
                "~/Scripts/bootstrap.min.js",
                "~/Scripts/jquery-ui-1.11.4.min.js",
                "~/Scripts//jquery-migrate-3.0.0.min.js",
                "~/Scripts/dynamitable.jquery.js",
                "~/Scripts/jexcel.js",
                "~/Scripts/jsuites.js",
                "~/Scripts/tinymce/tinymce.min.js",
                "~/Scripts/foundation.js",
                "~/Scripts/webapp.js"));

            // Use the Development version of Modernizr to develop with and learn from. Then, when you’re
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                "~/Scripts/modernizr-*"));

            /*
            ScriptManager.ScriptResourceMapping.AddDefinition(
                "respond",
                new ScriptResourceDefinition
                {
                    Path = "~/Scripts/respond.min.js",
                    DebugPath = "~/Scripts/respond.js",
                });
            */
        }
    }
}
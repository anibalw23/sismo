using System.Web;
using System.Web.Optimization;

namespace Monitoreo
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new StyleBundle("~/Content/css/styles").Include(
                      "~/Content/bootstrap.min.css",
                      "~/Content/bootstrap.theme.css",
                      "~/Content/css/languages.min.css",
                      "~/Content/css/styles.css"));



            bundles.Add(new ScriptBundle("~/bundles/bootstrap-datetimepicker/js").Include(
                      "~/Scripts/moment.js",
                      "~/Scripts/bootstrap-datetimepicker.js"));

            bundles.Add(new StyleBundle("~/bundles/bootstrap-datetimepicker/css/styles").Include(
                      "~/Content/bootstrap-datetimepicker.css"));

            bundles.Add(new ScriptBundle("~/Content/js/scripts").Include(
                      "~/Content/js/scripts.js"));


            // dataTables
            bundles.Add(new ScriptBundle("~/bundles/dataTables/js").Include(
                "~/Content/js/jquery.dataTables.js",
                "~/Content/js/dataTables.bootstrap.js"));

            bundles.Add(new StyleBundle("~/bundles/dataTables/css").Include(
                "~/Content/css/dataTables.bootstrap.css"));


            bundles.Add(new StyleBundle("~/bundles/select2/css/styles").Include(
                "~/Content/css/select2.css",
                "~/Content/css/select2-bootstrap.css"));

            bundles.Add(new ScriptBundle("~/bundles/select2/js").Include(
                "~/Scripts/select2.js",
                "~/Scripts/dataTables.bootstrap.js"));

        }
    }
}

using System.Web;
using System.Web.Optimization;

namespace MAC
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {

            bundles.Add(new ScriptBundle("~/admin-lte/js").Include(
             "~/Scripts/admin-lte/js/app.js",
             "~/Scripts/admin-lte/plugins/fastclick/fastclick.js",
             "~/Scripts/admin-lte/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js"
             ));
            
            bundles.Add(new StyleBundle("~/Content/css").Include(
                      "~/Content/bootstrap.css",
                      "~/Content/site.css",
                      "~/admin-lte/css/AdminLTE.css",
                      "~/admin-lte/css/skins/skin-blue.css",
                      "~/admin-lte/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.min.css",
                      "~/Content/font-awesome.css",
                      "~/Content/ionicons.css",
                      "~/Content/select2.min.css",
                      "~/Content/dataTables.bootstrap.min.css"
                      ));


            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/charts").Include(
                "~/Scripts/echarts.min.js",
                "~/Scripts/admin-lte/js/app.js",
                "~/Scripts/admin-lte/js/adminlte.js",
                "~/Scripts/admin-lte/plugins/fastclick/fastclick.js",
                "~/Scripts/admin-lte/plugins/bootstrap-wysihtml5/bootstrap3-wysihtml5.all.min.js",
                "~/Scripts/select2.full.min.js"));

            bundles.Add(new ScriptBundle("~/bundles/dataTables").Include(
                "~/Scripts/jquery.dataTables.min.js",
                "~/Scripts/dataTables.bootstrap.min.js",
                "~/Scripts/bootstrap-datepicker.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }
    }
}

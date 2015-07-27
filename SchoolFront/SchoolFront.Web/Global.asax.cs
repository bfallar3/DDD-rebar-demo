using System;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

using Accenture.Rebar.Web.Infrastructure.Helpers;

using Microsoft.Practices.EnterpriseLibrary.Logging;

namespace SchoolFront.Web
{
    /// <summary>
    /// Http application.
    /// </summary>
    public class MyHttpApplication : System.Web.HttpApplication
    {
        private static readonly LogWriter GlobalLogger = new LogWriterFactory().Create();
        /// <summary>
        /// Registers the global filters.
        /// </summary>
        /// <param name="filters">The filters.</param>
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            if (filters == null)
                throw new ArgumentNullException("filters");

            filters.Add(new HandleErrorAttribute());
        }

        /// <summary>
        /// Registers the routes.
        /// </summary>
        /// <param name="routes">The routes.</param>
        public static void RegisterRoutes(RouteCollection routes)
        {
            if (routes == null)
                throw new ArgumentNullException("routes");

            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional }); // Parameter defaults
        }

        /// <summary>
        /// Configuration to run at application start.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Can't be made static because this is the convention for wireing up events in the global.asax")]
        protected void Application_Start()
        {
            Logger.SetLogWriter(GlobalLogger);

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            /* 
             * Uncomment BundleTable.EnableOptimizations = true to force bundling/minification/cache-busting, useful for debugging
             * You may also want to change your Web settings in the Project Properties
             * to point to a vdir instead of root if that is how your app will be deployed
             * you may find a few issues with relative and absolute paths in that regard.
             *
             * We can also investigate using ~/ urls instead in the css since it will get
             * preprocessed by our module so absolute paths with the asp.net ~/ will actually
             * get converted correctly now
             */

            /* BundleTable.EnableOptimizations = true; */
            BundleConfig();
        }

        private static void BundleConfig()
        {
            var jqueryScripts = new ScriptBundle("~/ScriptBundle/jquery")
                .Include("~/Scripts/jquery-{version}.js");
            jqueryScripts.Transforms.Add(new JsMinify());

            var appScripts = new ScriptBundle("~/ScriptBundle/app")
                .IncludeDirectory("~/Scripts/infragistics", "*.js")
                .IncludeDirectory("~/Scripts/jqplot", "*.js", true)
                .Include("~/Scripts/jquery-ui-{version}.js")
                .Include("~/Scripts/jquery.*")
                .Include("~/Scripts/Site.js");
            appScripts.Transforms.Add(new JsMinify());

            /* Optional jQuery plugins. Include only the libraries you are using.
             * Don't include everything because the libraries can be large.
             * For example, if you use jQuery UI datepicker, include the library rebar.ui.datepicker.js.
             */
            var rebaruiScripts = new ScriptBundle("~/ScriptBundle/rebarui")
                .Include("~/Scripts/rebarUI/rebar.ui.datepicker.js")    /* jQuery UI Datepicker overrides */
                /* .Include("~/Scripts/rebarUI/rebar.ui.dialog.js") */        /* jQuery UI Dialog overrides */
                /* .Include("~/Scripts/rebarUI/rebar.ui.checkbox.js") */      /* Checkbox widget */
                /* .Include("~/Scripts/rebarUI/rebar.ui.radiobutton.js") */   /* Radiobutton widget */
                /* .Include("~/Scripts/rebarUI/rebar.ui.textbox.js") */       /* Textbox widget */
                /* .Include("~/Scripts/rebarUI/jquery.ui.selectmenu.js") */   /* Dropdownlist widget */
                /* .Include("~/Scripts/rebarUI/rebar.ui.selectlist.js") */    /* List widget */
                /* .Include("~/Scripts/rebarUI/rebar.ui.table.js") */         /* Table control */
                /* .Include("~/Scripts/rebarUI/rebar.ui.progressbar.js") */   /* jQuery UI Progressbar overrides */
                /* .Include("~/Scripts/rebarUI/rebar.ui.guidetext.js") */     /* Guidetext open source plugin */
                /* .Include("~/Scripts/rebarUI/rebar.ui.carousel.js") */      /* Media Carousel open source plugin */
                /* .Include("~/Scripts/rebarUI/rebar.ui.spinner.js") */       /* Wait Spinner open source plugin */
                /* .Include("~/Scripts/rebarUI/rebar.ui.slider-x.js") */;     /* Slider open source plugin */
            rebaruiScripts.Transforms.Add(new JsMinify());

            var customStyles = new StyleBundle("~/AssetBundles/Styles/Custom").Include(
                "~/Content/Styles/App/*.css", new VersionRewriteUrlTransform());
            customStyles.Transforms.Add(new CssMinify());

            var baseStyles = new StyleBundle("~/StyleBundle/Base").Include(
                "~/Content/Styles/themes/base/*.css", new VersionRewriteUrlTransform());
            baseStyles.Transforms.Add(new CssMinify());

            var redmondStyles = new StyleBundle("~/StyleBundle/Redmond").Include(
                "~/Content/Styles/themes/redmond/*.css", new VersionRewriteUrlTransform());
            redmondStyles.Transforms.Add(new CssMinify());

            var rebarUiStyles = new StyleBundle("~/StyleBundle/RebarUI").IncludeDirectory(
                "~/Content/Styles/themes/rebarUI", "*.css", true);
            rebarUiStyles.Transforms.Add(new CssMinify());

            BundleTable.Bundles.Add(jqueryScripts);
            BundleTable.Bundles.Add(rebaruiScripts);
            BundleTable.Bundles.Add(appScripts);

            BundleTable.Bundles.Add(customStyles);
            BundleTable.Bundles.Add(baseStyles);
            BundleTable.Bundles.Add(redmondStyles);
            BundleTable.Bundles.Add(rebarUiStyles);
        }
    }
}

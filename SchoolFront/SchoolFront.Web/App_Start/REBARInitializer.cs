using System.Configuration;
using System.Web.Mvc;
using System.Web.Routing;

using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.Configuration;
using Rebar.Web.Infrastructure;
using Rebar.Web.Infrastructure.Filters;
using SchoolFront.Web.ETW.Rebar.ETWLogging;

[assembly: WebActivatorEx.PreApplicationStartMethod(typeof(SchoolFront.Web.App_Start.RegisterRebarExtensions), "Prestart")]
[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(SchoolFront.Web.App_Start.RegisterRebarExtensions), "Start")]
[assembly: WebActivatorEx.ApplicationShutdownMethod(typeof(SchoolFront.Web.App_Start.RegisterRebarExtensions), "End")]

namespace SchoolFront.Web.App_Start
{
    /// <summary>
    /// Registers the REBAR MVC extensions.
    /// Code goes in here instead of directly into <c>global.asax</c> to support
    /// NuGet packaging.
    /// </summary>
    public static class RegisterRebarExtensions
    {
        private static RebarEventSource _logger = RebarEventSource.Log;
        /// <summary>
        /// Gets the Unity DI container.
        /// </summary>
        /// <value>
        /// The container.
        /// </value>
        public static IUnityContainer Container { get; private set; }

        /// <summary>
        /// Config run at prestart.
        /// </summary>
        public static void Prestart()
        {
            using (new EventContextScope())
            {
                _logger.ArchTraceLog("Starting application, creating the container");
            }
            CreateContainer();
        }

        /// <summary>
        /// Config run at start.
        /// </summary>
        public static void Start()
        {
            ControllerBuilder.Current.SetControllerFactory(new UnityControllerFactory(Container));

            RouteTable.Routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

            GlobalFilters.Filters.Add(new GlobalExceptionHandlingFilterAttribute());
        }

        /// <summary>
        /// Config run at end.
        /// </summary>
        public static void End()
        {
            using (new EventContextScope())
            {
                _logger.ArchTraceLog("Ending application, disposing of container");
            }
            if (Container != null)
            {
                Container.Dispose();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Container should live for the life of the application.")]
        private static void CreateContainer()
        {
            var container = new UnityContainer();
            var section = ConfigurationManager.GetSection("unity") as UnityConfigurationSection;
            if (section != null && section.Containers.Count > 0)
            {
                section.Configure(container);
            }

            Container = container;
        }
    }
}
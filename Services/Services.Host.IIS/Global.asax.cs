using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Rebar.CacheAdapter.Core;
using Rebar.CacheAdapter.MemoryCache;
using Rebar.Service.ExceptionHandling;
using Rebar.Service.Handlers;
using Rebar.Telemetry.Data;
using Rebar.Telemetry.EF6;

namespace Services.Host.IIS
{
    public class Global : System.Web.HttpApplication
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId = "Rebar.CacheAdapter.MemoryCache.MemoryCacheAdapter", Justification = "This instance would be used internally in CacheAdaptor, so the reference is not required in the calling method.")]
        protected void Application_Start(object sender, EventArgs e)
        {
            // If EF>=6 then use DataAccessEF6Profiler
            DataAccessEF6Profiler.Initialize();

            Logger.SetLogWriter(new LogWriterFactory().Create(), true);

            var config = GlobalConfiguration.Configuration;
            Services.Host.IIS.App_Start.TracingConfig.Register(config);
            IEnumerable<Func<DelegatingHandler>> perRouteServiceHandlers = new Func<DelegatingHandler>[] { () => new ClientIdHandler() };
            //To support basic case-insensitive parser behavior.
            config.EnableCaseInsensitive(caseInsensitive: true);
            // To support basic unqualified function/action call. 
            config.EnableUnqualifiedNameCall(unqualifiedNameCall: true);

            People.Service.Config.PeopleServiceRouteConfig.RegisterRoutes(config, perRouteServiceHandlers);
            School.Service.Config.SchoolServiceRouteConfig.RegisterRoutes(config, perRouteServiceHandlers);

            // ServiceConfigPlaceHolder - DO NOT DELETE THIS LINE

            // Set the Global exception filter for 500 errors.
            // Requires configuraiton for each exception type for shielding. 
            config.Filters.Add(new ExceptionHandlingAttribute());
            config.EnsureInitialized();

            // This registers local memory cache
            CacheGateway.Register<MemoryCacheAdapter>(cacheAdapter => new MemoryCacheAdapter());
        }

        protected void Session_Start(object sender, EventArgs e)
        {
        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
        }

        protected void Session_End(object sender, EventArgs e)
        {
        }

        protected void Application_End(object sender, EventArgs e)
        {
        }
    }
}

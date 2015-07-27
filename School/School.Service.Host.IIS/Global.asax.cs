using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData.Extensions;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Rebar.Service.ExceptionHandling;
using Rebar.Service.Handlers;
using Rebar.Telemetry.Data;

namespace School.ServiceHost.IIS
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            DataAccessProfiler.Initialize();
            Logger.SetLogWriter(new LogWriterFactory().Create(), true);

            var config = GlobalConfiguration.Configuration;
            School.Host.IIS.App_Start.TracingConfig.Register(config);
            IEnumerable<Func<DelegatingHandler>> perRouteServiceHandlers = new Func<DelegatingHandler>[] { () => new ClientIdHandler() };
            //To support basic case-insensitive parser behavior.
            config.EnableCaseInsensitive(caseInsensitive: true);
            // To support basic unqualified function/action call. 
            config.EnableUnqualifiedNameCall(unqualifiedNameCall: true);


            GlobalConfiguration.Configure(p => School.Service.Config.SchoolServiceRouteConfig.RegisterRoutes(config, perRouteServiceHandlers));

            // Set the Global exception filter for 500 errors.
            // Requires configuraiton for each exception type for shielding. 
            config.Filters.Add(new ExceptionHandlingAttribute());            
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
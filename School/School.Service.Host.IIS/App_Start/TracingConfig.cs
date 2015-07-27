using System;
using System.Web.Http;
using System.Web.Http.Tracing;
using Rebar.Tracing;

namespace School.Host.IIS.App_Start
{
    /// <summary>
    /// Tracing configuration
    /// </summary>
    public static class TracingConfig
    {
        /// <summary>
        /// Turns on tracing and replaces the default tracer with a custom RebarTraceWriter
        /// </summary>
        /// <param name="config"> HttpConfig to setup</param>
        public static void Register(HttpConfiguration config)
        {
            if (config == null)
                throw new ArgumentNullException("config", "Expected type HttpConfiguration");

            // Enable tracing
            config.EnableSystemDiagnosticsTracing();

            // Replace the default the writer with our own 
            config.Services.Replace(typeof(ITraceWriter), new RebarTraceWriter());
        }
    }
}
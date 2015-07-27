using System.Web.Routing;

using School.Host.IIS.App_Start;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(RegisterRebarWebHealthCheck), "PostApplicationStart")]

namespace School.Host.IIS.App_Start
{
    /// <summary>
    /// Registers the REBAR HealthCheck and Version HttpHandlers.
    /// Code goes in here instead of directly into <c>global.asax</c> to support
    /// NuGet packaging.
    /// </summary>
    public static class RegisterRebarWebHealthCheck
    {
        /// <summary>
        /// Calls made after application start.
        /// </summary>
        public static void PostApplicationStart()
        {
            var healthCheckHandler = new HealthCheckRouteHandler(/*optional params: endpoint name, isRemotelyCallable */);
            RouteTable.Routes.Insert(0, new Route("HealthCheck/{*all}", healthCheckHandler));
            RouteTable.Routes.Insert(0, new Route("Version/{*all}", new VersionRouteHandler()));
        }
    }
}
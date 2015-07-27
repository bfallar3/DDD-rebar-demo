using System.Web.Routing;

using School.Service.App_Start;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(RegisterRebarWebHealthCheck), "PostApplicationStart")]

namespace School.Service.App_Start
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
            var healthCheckHandler = new HealthCheckRouteHandler("School-service", true);
            RouteTable.Routes.Insert(
                0, new Route("School-service/HealthCheck/{*all}", healthCheckHandler));

            RouteTable.Routes.Insert(0, new Route("School-service/Version/{*all}", new VersionRouteHandler()));
        }
    }
}
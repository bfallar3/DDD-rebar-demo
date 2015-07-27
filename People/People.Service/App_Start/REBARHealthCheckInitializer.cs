using System.Web.Routing;

using People.Service.App_Start;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(RegisterRebarWebHealthCheck), "PostApplicationStart")]

namespace People.Service.App_Start
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
            var healthCheckHandler = new HealthCheckRouteHandler("People-service", true);
            RouteTable.Routes.Insert(
                0, new Route("People-service/HealthCheck/{*all}", healthCheckHandler));

            RouteTable.Routes.Insert(0, new Route("People-service/Version/{*all}", new VersionRouteHandler()));
        }
    }
}
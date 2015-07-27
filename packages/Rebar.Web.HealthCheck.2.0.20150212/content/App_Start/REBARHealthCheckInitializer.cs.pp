using System.Web.Routing;

using $rootnamespace$.App_Start;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(RegisterRebarWebHealthCheck), "PostApplicationStart")]

namespace $rootnamespace$.App_Start
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
            var healthCheckRoute = new Route("HealthCheck/{*all}", healthCheckHandler)
            {
                // we have to specify these, so no MVC route helpers will match, e.g. @Html.ActionLink("Home", "Index", "Home")
                Defaults = new RouteValueDictionary(new { controller = "HealthCheckRouteHandler" }),
                Constraints = new RouteValueDictionary(new { controller = "HealthCheckRouteHandler" })
            };
            RouteTable.Routes.Insert(0, healthCheckRoute);
 
            var versionRoute = new Route("Version/{*all}", new VersionRouteHandler())
            {
                // we have to specify these, so no MVC route helpers will match, e.g. @Html.ActionLink("Home", "Index", "Home")
                Defaults = new RouteValueDictionary(new { controller = "VersionRouteHandler" }),
                Constraints = new RouteValueDictionary(new { controller = "VersionRouteHandler" })
            };
            RouteTable.Routes.Insert(0, versionRoute);
        }
    }
}
using System;
using System.Web;
using System.Web.Routing;
using Rebar.Web.HealthCheck;

namespace $rootnamespace$.App_Start
{
    /// <summary>
    /// Registers the version route
    /// </summary>
    public class VersionRouteHandler : IRouteHandler
    {
        /// <summary>
        /// The handler
        /// </summary>
        private readonly Lazy<IHttpHandler> handler = new Lazy<IHttpHandler>(CreateHandler);

        /// <summary>
        /// Provides the object that processes the request.
        /// </summary>
        /// <param name="requestContext">An object that encapsulates information about the request.</param>
        /// <returns>An object that processes the request.</returns>
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return this.handler.Value;
        }

        /// <summary>
        /// Creates the handler.
        /// </summary>
        /// <returns>The handler</returns>
        private static IHttpHandler CreateHandler()
        {
            return new VersionAsyncHandler();
        }
    }
}
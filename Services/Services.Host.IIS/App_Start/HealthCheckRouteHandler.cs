using System;
using System.Web;
using System.Web.Routing;
using Rebar.Web.HealthCheck;

namespace Services.Host.IIS.App_Start
{
    /// <summary>
    /// Registers the health check route
    /// </summary>
    public class HealthCheckRouteHandler : IRouteHandler
    {
        private static string endpointId;
        private static bool isRemotelyCallable; 
        
        private readonly Lazy<IHttpHandler> handler = new Lazy<IHttpHandler>(CreateHandler);

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckRouteHandler"/> class.
        /// </summary>
        /// <param name="endpointId">The endpoint id.</param>
        /// <param name="isRemotelyCallable">if set to <c>true</c> [is remotely callable].</param>
        public HealthCheckRouteHandler(string endpointId = null, bool isRemotelyCallable = false)
        {
            HealthCheckRouteHandler.endpointId = endpointId;
            HealthCheckRouteHandler.isRemotelyCallable = isRemotelyCallable;
            if (isRemotelyCallable)
            {
                // force loading for child health check
                CreateHandler();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HealthCheckRouteHandler"/> class.
        /// </summary>
        public HealthCheckRouteHandler()
        {
        }

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
            var handler = new HealthCheckAsyncHandler(endpointId, isRemotelyCallable);
            return handler;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;
using System.Web.OData.Extensions;

using Accenture.Security.Eso.Service;
using Microsoft.OData.Edm;
using Thinktecture.IdentityModel.Tokens.Http;
using System.Web.OData.Routing;
using System.Web.OData.Routing.Conventions;

namespace Rebar.Web
{
    /// <summary>
    /// Class ODataRouteConfigurationExtensions
    /// </summary>
    public static class ODataRouteConfigurationExtensions
    {
        internal const string DisableSecuritySetting = "Rebar.Server.DisableLocalSecurity";

        /// <summary>
        /// Maps a route to a service.
        /// </summary>
        /// <param name="config">The config.</param>
        /// <param name="routeName">Name of the route.</param>
        /// <param name="routePrefix">The route prefix.</param>
        /// <param name="model">The model.</param>
        /// <param name="handlers">The handlers.</param>
        /// <param name="serviceIdentifier">The security identifier for the service</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Suppression OK, need a way to pass around constructors")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Suppression OK, framework will dispose of this.")]
        public static void MapRebarODataRoute(
            this HttpConfiguration config,
            string routeName,
            string routePrefix,
            IEdmModel model,
            IEnumerable<Func<DelegatingHandler>> handlers,
            string serviceIdentifier = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            HttpMessageHandler delegatingHandler;

            if (handlers != null)
            {
                delegatingHandler = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), handlers.Select(x => x()));
            }
            else
            {
                delegatingHandler = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), null);
            }

            DelegatingHandler handler;
            var uriBuilder = new UriBuilder();

            var disableSecurity = ConfigurationManager.AppSettings[DisableSecuritySetting];
            var isSecurityDisabled = !string.IsNullOrWhiteSpace(disableSecurity) && bool.Parse(disableSecurity);
            if (uriBuilder.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase) && isSecurityDisabled)
            {
                handler = new EmptyAuthenticationHandler(delegatingHandler);
            }
            else
            {
                var identifier = serviceIdentifier ?? ((NameValueCollection)ConfigurationManager.GetSection("accenture.security.eso.service"))["Services:Identifier"];
                var authConfig = new AuthenticationConfiguration { RequireSsl = false, SetPrincipalOnRequestInstance = true };
                authConfig.AddMsftJsonWebToken(identifier);

                handler = new AuthenticationHandler(authConfig, delegatingHandler);
            }

            // Create the default odata route using regular conventions
            config.MapODataServiceRoute(
                         routeName: routeName,
                         routePrefix: routePrefix,
                         model: model,
                         pathHandler: new DefaultODataPathHandler(),
                         routingConventions: ODataRoutingConventions.CreateDefaultWithAttributeRouting(config, model),
                         defaultHandler: handler);

                      

        }
    }
}
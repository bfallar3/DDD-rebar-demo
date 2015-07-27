using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Builder;

using Rebar.Web;

namespace School.Service.Config
{
    /// <summary>The School service route config.</summary>
    public static class SchoolServiceRouteConfig
    {
        /// <summary>Registers the routes.</summary>
        /// <param name="config">The config.</param>
        /// <param name="handlers">The delegating handlers</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Suppression OK, need a generic way to pass around constructors")]
        public static void RegisterRoutes(HttpConfiguration config, IEnumerable<Func<DelegatingHandler>> handlers)
        {
            if (config == null)
                throw new ArgumentNullException("config");

            /*
             * NOTES:
             * RegisterRoutes handles the following:
             * * Creates an Entity Data Model (EDM) for the OData endpoint
             * * Adds the School entity set to the EDM
             * * - Entities.School is our POCO entity class defined in CostCollector.cs.
             * * - "School" is the name of the entity set IDbSet<School> defined in SchoolModelContainer.cs.
             * * Sets up the OData URIs and configures the endpoint
             */

            const string ServiceUriPrefix = "School-service";
            const string EntitySetName = "School";

            ODataModelBuilder modelBuilder = new ODataConventionModelBuilder();
            var entitySet = modelBuilder.EntitySet<Entities.School>(EntitySetName);
            modelBuilder.Namespace = "School.Service.Entities";
            entitySet.HasFeedSelfLink(
                x =>
                    {
                        var linkUri = new UriBuilder(x.Request.RequestUri).AddFiddler();
                        return linkUri.Uri;
                    });

            entitySet.HasReadLink(x => LinkFactory(x, ServiceUriPrefix, EntitySetName), true);

            entitySet.HasIdLink(x => LinkFactory(x, ServiceUriPrefix, EntitySetName), true);

            entitySet.HasEditLink(x => LinkFactory(x, ServiceUriPrefix, EntitySetName), true);

            var model = modelBuilder.GetEdmModel();

            /*
             * Be aware that the name of the controller must match the name of the entity set. That means that there must be a 
             * SchoolController.
             * 
             * Our URI will be ~/School-service.
             */
            config.MapRebarODataRoute("SchoolService-OData", ServiceUriPrefix, model, handlers);
        }

        private static Uri LinkFactory(EntityInstanceContext<Entities.School> context, string ServiceUriPrefix, string EntitySetName)
        {
            var linkUri = context.Request.RequestUri.FindParentUri(ServiceUriPrefix);
            object id = new object();
            context.EdmObject.TryGetPropertyValue("Id", out id);
            linkUri.Query = string.Empty;
            linkUri.Fragment = string.Empty;
            linkUri.Path = string.Format(
              CultureInfo.InvariantCulture,
              "{0}{1}({2})",
              linkUri.Path,
              EntitySetName,
              (int)id);
              
            linkUri.AddFiddler();
            return linkUri.Uri;
        }
    }
}

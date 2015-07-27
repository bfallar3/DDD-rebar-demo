using System;
using System.Data.Services.Client;
using System.Web;
using SchoolFront.Web.Properties;
using Accenture.Rebar.Web;
using Accenture.Rebar.Web.Infrastructure.Helpers;
using ODataV4Client = Microsoft.OData.Client;
////using ExampleService = SchoolFront.Web.ExampleService.V1_0_0;

namespace SchoolFront.Web.Infrastructure.Service
{
    /// <summary>
    /// The Service Factory class centralizes the creation and initialization of service entity containers
    /// </summary>
    public static class ServiceFactory
    {
        /// <summary>
        /// Creates the container for the Service Entity.
        /// A CreateContainer method should be created for each service that your web app will reference.
        /// This will require a reference to the Service.  Add a service reference by using the local $metadata URI of your service.
        /// </summary>
        /// <returns>The Service Entity container.</returns>
        /*
        public static ExampleService.Container CreateServiceContainer()
        {
            return
                InitializeContext(
                    new ExampleService.Container(
                        new Uri(Settings.Default.ExampleServiceUri).AddFiddler()),
                            "SchoolFrontservices");
        }
        */

        /// <summary>
        /// Creates the container for the ODataV4 Service Entity 
        /// A CreateContainer method should be created for each service that your web app will reference.
        /// This will require a reference to the Service.  Add a service reference by using the local $metadata URI of your service.
        /// </summary>
        /// <returns>The Service Entity container.</returns>
        /*
        public static ExampleService.Container CreateServiceContainer()
        {
            return
                InitializeContextForODataV4Service(
                    new ExampleService.Container(
                        new Uri(Settings.Default.ExampleServiceUri).AddFiddler()),
                            "SchoolFrontservices");
        }
        */

        /// <summary>
        /// Initializes Service Context
        /// </summary>
        /// <typeparam name="T">Service Context Type</typeparam>
        /// <param name="context">Service Proxy Container</param>
        /// <param name="sid">Service Identifier</param>
        /// <returns>Service Context</returns>
        private static T InitializeContext<T>(T context, string sid) where T : DataServiceContext
        {
            /* 
             * Adding clientId to service requests.  
             * Without clientId, the service call will be rejected by the service.
             * Jwt token is required for ADFS authentication.  The jwt token is cached and renewed upon 401 authorization. 
             */

            var jwtToken = RebarSecurityToken.GetToken(sid, HttpContext.Current.User.Identity.Name);
            
            context.SendingRequest2 += (sender, args) => args.RequestMessage.SetHeader("x-acc-clientid", "{AIRID_SchoolFront_MVC}");

            context.SendingRequest2 += (sender, args) => args.RequestMessage.SetHeader("Authorization", "Bearer " + jwtToken);
           
            return context;
        }

        /// <summary>
        /// Initializes Service Context
        /// </summary>
        /// <typeparam name="T">Service Context Type</typeparam>
        /// <param name="context">Service Proxy Container</param>
        /// <param name="sid">Service Identifier</param>
        /// <returns>Service Context</returns>
        private static T InitializeContextForODataV4Service<T>(T context, string sid) where T : ODataV4Client.DataServiceContext
        {
            /* 
             * Adding clientId to service requests.  
             * Without clientId, the service call will be rejected by the service.
             * Jwt token is required for ADFS authentication.  The jwt token is cached and renewed upon 401 authorization. 
             */

            var jwtToken = RebarSecurityToken.GetToken(sid, HttpContext.Current.User.Identity.Name);

            context.SendingRequest2 += (sender, args) => args.RequestMessage.SetHeader("x-acc-clientid", "{AIRID_SchoolFront_MVC}");

            context.SendingRequest2 += (sender, args) => args.RequestMessage.SetHeader("Authorization", "Bearer " + jwtToken);

            return context;
        }
    }
}
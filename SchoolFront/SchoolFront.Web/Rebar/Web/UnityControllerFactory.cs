using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Practices.Unity;
using Rebar.Web.Infrastructure.Properties;

namespace Rebar.Web.Infrastructure
{
    /// <summary>
    /// Unity Controller Factory
    /// </summary>
    public class UnityControllerFactory : DefaultControllerFactory
    {
        private readonly IUnityContainer container;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnityControllerFactory" /> class.
        /// </summary>
        /// <param name="container">Unity Container</param>
        public UnityControllerFactory(IUnityContainer container)
        {
            ////Guard.NotNull("container", container);
            this.container = container;
        }

        /// <summary>
        /// Returns an instance of the Unity Controller
        /// </summary>
        /// <param name="requestContext">Request context</param>
        /// <param name="controllerType">Controller type</param>
        /// <returns>Appropriate Unity controller based on context and type</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.String.Format(System.String,System.Object)", Justification = "Reviewed. Supression OK here.")]
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (requestContext == null)
                throw new ArgumentNullException("requestContext");

            IController controller = null;
            if (controllerType == null)
            {
                requestContext.HttpContext.Response.TrySkipIisCustomErrors = true;
                throw new HttpException(404, string.Format(Resources.Controller_For_Path_Not_Found_Format, requestContext.HttpContext.Request.Path));
            }

            if (!typeof(IController).IsAssignableFrom(controllerType))
                throw new ArgumentException(
                        string.Format(
                            "Type requested is not a controller: {0}",
                            controllerType.Name),
                            "controllerType");
            try
            {
                controller = this.container.Resolve(controllerType) as IController;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    string.Format("Error resolving controller {0}", controllerType.Name), ex);
            }

            return controller;
        }
    }
}
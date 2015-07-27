using System.Web.Mvc;

namespace Accenture.SchoolFront.Web.Controllers
{
    /// <summary>Provides common functionality for controllers.</summary>
    public class BaseController : Controller
    {
        /// <summary>
        /// Uses Reflection to Load the assembly version of the current application. Result is set to a 24 hour cache to minimize amount of reflection calls.
        /// </summary>
        /// <returns>String Number of the Actual version of the application</returns>
        [OutputCache(Duration = 86400, VaryByParam = "none")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Supression Ok Here, HTTP GET Controller Method")]
        public virtual string GetApplicationVersion()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;

            return version == null ? "VERSION UNKNOWN" : version.ToString();
        }

        /// <summary>Gets the name of the parent controller from the value passed into the parent request's route data values.</summary>
        /// <returns>The name of the parent controller, or the current controller if no parent exists, or empty if controller context is null</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Supression Ok Here, HTTP GET Controller Method")]
        protected virtual string GetParentControllerNameOrDefault()
        {
            if (this.ControllerContext != null)
            {
                if (ControllerContext.ParentActionViewContext != null && ControllerContext.ParentActionViewContext.Controller != null)
                {
                    return ControllerContext.ParentActionViewContext.RouteData.Values["controller"].ToString();
                }

                return ControllerContext.RouteData.Values["controller"].ToString();
            }

            return string.Empty;
        }
    }
}

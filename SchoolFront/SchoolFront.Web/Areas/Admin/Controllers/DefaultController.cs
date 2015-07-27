using System.Web.Mvc;

using Accenture.Security.Eso.Web;

namespace SchoolFront.Web.Areas.Admin.Controllers
{
    /// <summary>
    /// Example controller.
    /// </summary>
    public class DefaultController : Controller
    {
        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns>
        /// The <see cref="ActionResult"/>.
        /// </returns>
        [Authorize]
        public ActionResult Index()
        {
            var identity = (IEnterpriseIdentity)User.Identity;
            ViewBag.DisplayName = identity.DisplayName;
            return this.View();
        }
    }
}

using System;
using System.Linq;
using System.Diagnostics;
using System.IdentityModel.Services;
using System.Web.Mvc;

using School = SchoolFront.Web.Proxy.School;
using People = SchoolFront.Web.Proxy.People;

namespace Accenture.SchoolFront.Web.Controllers
{
    /// <summary>
    /// Home controller.
    /// </summary>
    public class HomeController : BaseController
    {
        /// <summary>
        /// The index action.
        /// </summary>
        /// <returns>The ActionResult.</returns>
        public ActionResult Index()
        {
            var schoolContainer = new School.Container(new Uri("http://localhost:10600/School-service/"));
            var peopleContainer = new People.Container(new Uri("http://localhost:10600/People-service/"));

            var s = schoolContainer.School.ToList();

            return this.View();
        }

        /// <summary>
        /// For Authentication.
        /// The LogOn action.
        /// </summary>
        /// <returns>The ActionResult.</returns>
        public ActionResult LogOn()
        {
            return this.View();
        }

        /// <summary>
        /// The LogOff action.
        /// </summary>
        /// <returns>The ActionResult.</returns>
        public ActionResult LogOff()
        {
            FederatedAuthentication.SessionAuthenticationModule.SignOut();
            return this.Redirect("/");
        }
    }
}
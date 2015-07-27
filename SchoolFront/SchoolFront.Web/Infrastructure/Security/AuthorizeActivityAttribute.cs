using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

using Accenture.Security.Eso.Web;

namespace SchoolFront.Web.Infrastructure.Security
{
    /// <summary>
    /// The authorized activity attribute.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)] 
    public sealed class AuthorizeActivityAttribute : AuthorizeAttribute
    {
        private static readonly Dictionary<Activities, IEnumerable<Groups>> ActivitiesThatGroupsCanPerform = 
            AuthorizationMapping.ActivitiesThatGroupsCanPerform().ToDictionary(x => x.Key, x => x.Value);

        /// <summary>
        /// Gets or sets the allowed roles to this property.
        /// </summary>
        public Activities Is { get; set; }

        /// <summary>
        /// Called when a process requests authorization.
        /// </summary>
        /// <param name="filterContext">The filter context, which encapsulates information for using <see cref="T:System.Web.Mvc.AuthorizeAttribute" />.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Can't be avoided.")]
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            var enterprisePrincipal = (IEnterprisePrincipal)filterContext.HttpContext.User;

            var groups = enterprisePrincipal.EnterpriseIdentity
                            .GetGroups()
                            .Select(s => Enum.Parse(typeof(Groups), s))
                            .ToArray();

            // Is == 0 if no activities were specified
            if (this.Is != 0) 
            {
                var validGroups = ActivitiesThatGroupsCanPerform
                    .Where(activity => (this.Is & activity.Key) == activity.Key)
                    .SelectMany(x => x.Value);

                /* Here we take the groups the user is a member of and create a list of type Groups
                   so we can then perform a bitwise comparison in the following IF statement */
                Groups validGroupsDistinct = validGroups.Distinct().Aggregate((a, b) => a | b);

                /* Now we look for any matching groups in these activities
                   any match means this user is authorized to access the activity */
                if (groups.Cast<Groups>().Any(group => ((validGroupsDistinct & group) == group)))
                    return;
            }

            // user is not authorized for the given activity
            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}
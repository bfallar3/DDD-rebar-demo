using System;
using System.Globalization;
using System.Security.Principal;

using Accenture.Security.Eso.Web;

namespace SchoolFront.Web.Infrastructure.Security
{
    /// <summary>
    /// Identity extension methods.
    /// </summary>
    public static class IdentityExtensions
    {
        /// <summary>
        /// Gets the people key.
        /// </summary>
        /// <param name="principal">The principal.</param>
        /// <returns>The people key.</returns>
        /// <exception cref="System.ArgumentException">The principal's identity is not convertible to an IEnterpriseIdentity;principal</exception>
        public static int GetPeopleKey(this IPrincipal principal)
        {
            if (principal == null)
                throw new ArgumentNullException("principal");

            var enterpriseIdentity = principal.Identity as IEnterpriseIdentity;

            if (enterpriseIdentity == null)
                throw new ArgumentException("The principal's identity is not convertible to an IEnterpriseIdentity", "principal");

            return Convert.ToInt32(enterpriseIdentity.PeopleKey, CultureInfo.InvariantCulture);
        }
    }
}
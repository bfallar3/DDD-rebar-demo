using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SchoolFront.Web.Infrastructure.Security
{
    /// <summary>
    /// Activity to group authorization mapping.
    /// </summary>
    public static class AuthorizationMapping
    {
        /// <summary>
        /// Activities that the groups can perform.
        /// </summary>
        /// <returns>
        /// The activities that groups can perform.
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Reviewed. Suppresson Ok here.")]
        public static IEnumerable<KeyValuePair<Activities, IEnumerable<Groups>>> ActivitiesThatGroupsCanPerform()
        {
            return new Dictionary<Activities, IEnumerable<Groups>>
                {
                    { Activities.BusinessUse, Enum.GetValues(typeof(Groups)).Cast<Groups>().ToList().AsReadOnly() },
                    { Activities.EditSystemSettings, new List<Groups> { Groups.Admin }.AsReadOnly() },
                    { Activities.ViewSystemSettings, new List<Groups> { Groups.Admin, Groups.SuperUser }.AsReadOnly() }
                };
        }
    }
}
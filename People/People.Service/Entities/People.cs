using System;

using Rebar.EF.AuditPlugin;

namespace People.Service.Entities
{
    /// <summary>
    /// Entity to store People
    /// </summary>    
    public class People : IAuditCreations, IAuditMutations
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="People"/> class.
        /// </summary>
        public People()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="People"/> class.
        /// </summary>
        /// <param name="id">The ID.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "Entity Framework proxies requrie it.")]
        public People(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Gets or sets Id column.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the created date time.
        /// </summary>
        /// <value>
        /// The created date time.
        /// </value>
        public DateTimeOffset CreatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the created user.
        /// </summary>
        /// <value>
        /// The created user.
        /// </value>
        public string CreatedUser { get; set; }

        /// <summary>
        /// Gets or sets the updated date time.
        /// </summary>
        /// <value>
        /// The updated date time.
        /// </value>
        public DateTimeOffset UpdatedDateTime { get; set; }

        /// <summary>
        /// Gets or sets the version.
        /// </summary>
        /// <value>
        /// The version.
        /// </value>
        public int Version { get; set; }

        /// <summary>
        /// Gets or sets the updated user.
        /// </summary>
        /// <value>
        /// The updated user.
        /// </value>
        public string UpdatedUser { get; set; }
    }
}

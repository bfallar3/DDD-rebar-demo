using System.Data.Entity.ModelConfiguration;

namespace People.Service.Entities
{
    /// <summary>
    /// The DB mapping configuration for the TimeReport entity.
    /// </summary>
    public class PeopleMapping : EntityTypeConfiguration<People>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeopleMapping"/> class.
        /// </summary>
        public PeopleMapping()
        {
            /*
             * This is how to map the POCO People class to its persisted DB
             * form using Entity Framework.
             * 
             * Note that while EF also supports annotations directly on the POCO TimeReport class,
             * it's better to keep your POCO entity class clean and consolidate all the EF-related
             * configuration here in a single location.
             */

            this.HasKey(t => t.Id);
            this.ToTable("People");

            this.Property(t => t.Id).IsRequired();

            this.Property(t => t.Version).IsConcurrencyToken();
            this.Property(t => t.CreatedDateTime).HasColumnType("datetimeoffset");
            this.Property(t => t.UpdatedDateTime).HasColumnType("datetimeoffset");
        }
    }
}

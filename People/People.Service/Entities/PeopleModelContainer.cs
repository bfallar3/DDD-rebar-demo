using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.CodeAnalysis;

using Rebar.EF.PluginManager;

namespace People.Service.Entities
{
    /// <summary>The CostCollector Model Container.</summary>
    public class PeopleModelContainer : PluggableModelContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeopleModelContainer"/> class.
        /// </summary>
        public PeopleModelContainer()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PeopleModelContainer"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        public PeopleModelContainer(string connectionString)
            : base(connectionString)
        {
        }

        /// <summary>
        /// Gets or sets PeopleSet
        /// </summary>
        public IDbSet<People> PeopleSet { get; set; }

        /// <summary>
        /// This method is called when the model for a derived context has been initialized, but
        /// before the model has been locked down and used to initialize the context.  The default
        /// implementation of this method does nothing, but it can be overridden in a derived class
        /// such that the model can be further configured before it is locked down.
        /// </summary>
        /// <param name="modelBuilder">The builder that defines the model for the context being created.</param>
        /// <remarks>
        /// Typically, this method is called only once when the first instance of a derived context
        /// is created.  The model for that context is then cached and is for all further instances of
        /// the context in the app domain.  This caching can be disabled by setting the ModelCaching
        /// property on the given ModelBuilder, but note that this can seriously degrade performance.
        /// More control over caching is provided through use of the DbModelBuilder and DbContextFactory
        /// classes directly.
        /// </remarks>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed. Suppression is OK here.")]
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            /*
             * While you can add your EF configuration directly in the OnModelCreating method,
             * it's better to create a separate class for each entity.
             * That way, if you have multiple entities and/or complex mapping, you can more easily
             * organize the configuration code.
             */

            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            modelBuilder.Configurations.Add(new PeopleMapping());
            base.OnModelCreating(modelBuilder);
        }
    }
}

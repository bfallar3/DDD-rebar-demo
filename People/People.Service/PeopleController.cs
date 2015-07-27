using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;

using Rebar.CacheAdapter;
using Rebar.CacheAdapter.Core;
using Rebar.EF.PluginManager;

using Entities = People.Service.Entities;

namespace People.Service
{
    /// <summary>
    /// PeopleController is a default controller implementation for your WebAPI OData service. 
    /// Modify it to meet your requirements.
    /// </summary>
    [Authorize]
    public class PeopleController : ODataController
    {
        private readonly Entities.PeopleModelContainer context = DbContextFactory<Entities.PeopleModelContainer>.Create();

        /// <summary>
        /// Gets this instance.
        /// </summary>
        [EnableQuery(PageSize = 10)]
        public IQueryable<Entities.People> Get()
        {
            /*
             * Note the [Queryable(PageSize = 10)] annotation above.
             * This constrains the result returned to no more than 10 records.
             * Clients must do page-by in order to page through the data set.
             */

            var result = this.context.PeopleSet;
            return result;
        }

        /// <summary>Gets a People by Id.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The People.</returns>
        public Entities.People Get(int key)
        {
            var cacheKey = new CacheKeyBuilder("People.Service.V1_0_0").New(key);

            var result = CacheGateway.Instance()
                                     .Get(cacheKey, () => this.context.PeopleSet.FirstOrDefault(x => x.Id == key));

            return result;
        }

        /// <summary>Update Method For Users</summary>
        /// <param name="key">Entity key</param>
        /// <param name="update">User Entity to update</param>
        /// <returns>returns updated User</returns>
        public Entities.People Put(int key, Entities.People update)
        {
            if (update == null)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (key != update.Id)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (!ModelState.IsValid)
            {
                throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            if (!this.context.PeopleSet.Any(p => p.Id == key))
            {
                throw new HttpResponseException(HttpStatusCode.NotFound);
            }

            this.context.PeopleSet.Attach(update);
            this.context.Entry(update).State = System.Data.Entity.EntityState.Modified;
            try
            {
                this.context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new HttpResponseException(HttpStatusCode.Conflict);
            }

            return update;
        }

        /// <summary>Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            this.context.Dispose();
            base.Dispose(disposing);
        }
    }
}
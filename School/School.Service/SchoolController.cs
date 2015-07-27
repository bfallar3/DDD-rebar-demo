using Rebar.EF.PluginManager;
using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.OData;

namespace School.Service
{
    /// <summary>
    /// SchoolController is a default controller implementation for your WebAPI OData service. 
    /// Modify it to meet your requirements.
    /// </summary>
    [Authorize]
    public class SchoolController : ODataController
    {
        private readonly Entities.SchoolModelContainer context = DbContextFactory<Entities.SchoolModelContainer>.Create("SchoolContext");

        /// <summary>
        /// Gets this instance.
        /// </summary>
        [EnableQuery(PageSize = 10)]
        public IQueryable<Entities.School> Get ()
        {
            /*
             * Note the [Queryable(PageSize = 10)] annotation above.
             * This constrains the result returned to no more than 10 records.
             * Clients must do page-by in order to page through the data set.
             */

            var result = from c in context.Courses
                         select new Entities.School
                         {
                             CreatedDateTime = DateTime.Now,
                             CreatedUser = "b.p.fallar",
                             UpdatedDateTime = DateTime.Now,
                             UpdatedUser = "b.p.fallar",
                             Id = c.CourseID,
                             Version = 1
                         };

            return result;
        }

        /// <summary>Gets a School by Id.</summary>
        /// <param name="key">The key.</param>
        /// <returns>The School.</returns>
        public Entities.School Get (int key)
        {
            var result = (from c in context.Courses
                          select new Entities.School
                          {
                              CreatedDateTime = DateTime.Now,
                              CreatedUser = "b.p.fallar",
                              UpdatedDateTime = DateTime.Now,
                              UpdatedUser = "b.p.fallar",
                              Id = c.CourseID,
                              Version = 1
                          }).FirstOrDefault();
            return result;
        }

        /// <summary>Update Method For Users</summary>
        /// <param name="key">Entity key</param>
        /// <param name="update">User Entity to update</param>
        /// <returns>returns updated User</returns>
        public Entities.School Put (int key, Entities.School update)
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

            //if (!this.context.SchoolSet.Any(p => p.Id == key))
            //{
            //    throw new HttpResponseException(HttpStatusCode.NotFound);
            //}

            //this.context.SchoolSet.Attach(update);
            //this.context.Entry(update).State = System.Data.Entity.EntityState.Modified;
            //try
            //{
            //    this.context.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    throw new HttpResponseException(HttpStatusCode.Conflict);
            //}

            return update;
        }

        /// <summary>Releases the unmanaged resources that are used by the object and, optionally, releases the managed resources.</summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
        protected override void Dispose (bool disposing)
        {
            this.context.Dispose();
            base.Dispose(disposing);
        }
    }
}
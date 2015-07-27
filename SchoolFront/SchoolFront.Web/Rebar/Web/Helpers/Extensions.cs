using System;
using System.Diagnostics;

namespace Accenture.Rebar.Web.Infrastructure.Helpers
{
    /// <summary>
    /// Extension methods
    /// </summary>
    public static class Extensions
    {
        /// <summary>Safely invokes the expression, catching a <see cref="NullReferenceException"/> and returning <paramref name="nullValue"/></summary>
        /// <typeparam name="TModel">The type of the model.</typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="model">The model.</param>
        /// <param name="expression">The expression.</param>
        /// <param name="nullValue">The null value.</param>
        /// <returns>The result of the <paramref name="expression"/> or the <paramref name="nullValue"/> if a <see cref="NullReferenceException"/> is thrown.</returns>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// user.Location = null;
        /// 
        /// TRADITIONAL WAY WITH TERNARY OPERATOR NULL CHECKS
        /// model.City = user.Location != null ? user.Location.City : null;
        /// Console.WriteLine(model.City) // null
        /// 
        /// model.City = user.SafeInvoke( x => x.Location.City );
        /// Console.WriteLine(model.City) // null
        /// 
        /// model.City = user.SafeInvoke( x => x.Location.City, "Chicago" );
        /// Console.WriteLine(model.City) // Chicago
        /// 
        /// NESTED EXPRESSIONS
        /// model.ReviewerCity = user.Reviewer != null && user.Reviewer.Location != null ? user.Reviewer.Location.City : null;
        /// model.ReviewerCity = user.SafeInvoke( x => x.Reviewer.Location.City );
        /// ]]>
        /// </code>
        /// </example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed", Justification = "Reviewed. Suppression Ok here.")]
        public static TResult SafeInvoke<TModel, TResult>(this TModel model, Func<TModel, TResult> expression, TResult nullValue = default(TResult))
        {
            if (expression == null) return nullValue;

            try
            {
                return expression(model);
            }
            catch (NullReferenceException)
            {
                return nullValue;
            }
        }

        /// <summary>
        /// Adds .fiddler to the host name if Fiddler is running.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns>The modified Uri.</returns>
        public static Uri AddFiddler(this Uri uri)
        {
            if (uri == null) throw new ArgumentNullException("uri");
            if (!uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return uri;

            var processes = Process.GetProcessesByName("Fiddler");
            if (processes.Length > 0)
            {
                var uriBuilder = new UriBuilder(uri) { Host = uri.Host + ".fiddler" };
                return new Uri(uriBuilder.ToString());
            }

            return uri;
        }
    }
}
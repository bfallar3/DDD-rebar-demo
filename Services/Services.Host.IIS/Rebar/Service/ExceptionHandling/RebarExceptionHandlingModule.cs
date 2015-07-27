using System;
using System.Globalization;
using System.Web;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

namespace Rebar.Service.ExceptionHandling
{
    /// <summary>
    /// Rebar exception handling module.
    /// </summary>
    public class RebarExceptionHandlingModule : IHttpModule
    {
        private static ExceptionManager exceptionManager;
        /// <summary>
        /// Dispose Method for the handlers
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Automatically called by IIS on every request
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpApplication" /> that provides access to the methods, properties, and events common to all application objects within an ASP.NET application</param>
        /// <exception cref="System.ArgumentNullException">the HTTP context</exception>
        public void Init(HttpApplication context)
        {
            if (context == null) throw new ArgumentNullException("context");

            exceptionManager = new ExceptionPolicyFactory().CreateManager();
            context.EndRequest += this.OnError;
        }

        /// <summary>
        /// EventHandler for request
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="e">Event raised</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry.AddErrorMessage(System.String)", Justification = "Intentional.")]
        public void OnError(object value, EventArgs e)
        {
            var ctx = HttpContext.Current;
            var exception = ctx.Server.GetLastError();

            if (exception == null)
            {
                // Log Any and all 400 errors
                if (ctx.Response.StatusCode > 399 && ctx.Response.StatusCode < 500)
                {
                    if (ctx.Request.FilePath.ToUpperInvariant() != "/favicon.ico".ToUpperInvariant())
                    {
                        exceptionManager.HandleException(GetException(ctx), "Global Exception Policy");
                    }
                }
                else if (ctx.Response.StatusCode > 499)
                {
                    // Log Any 500 errors
                    // If the Header contains an ErrorId then its already been handled
                    if (string.IsNullOrEmpty(ctx.Response.Headers["ErrorId"]))
                    {
                        exceptionManager.HandleException(GetException(ctx), "Global Exception Policy");
                    }
                }
            }
        }

        /// <summary>
        /// Generates a new exception from the Context. Because the context does not contain an error for a 400 or 500 error due to being already processed by OData, its generating whatever data it has, such as route, status code, status
        /// </summary>
        /// <param name="context">Current Context</param>
        /// <returns>
        /// new Exception
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2201:DoNotRaiseReservedExceptionTypes", Justification = "Intentional.")]
        protected static Exception GetException(HttpContext context)
        {
            if (context == null) throw new ArgumentNullException("context");

            var fault = new Exception(string.Format(CultureInfo.InvariantCulture, "uri :{0}\nError Code: {1}\nError: {2}\nDescription :{3}", context.Request.Url.AbsoluteUri, context.Response.StatusCode, context.Response.Status, context.Response.StatusDescription));
                        
            return fault;
        }
    }
}